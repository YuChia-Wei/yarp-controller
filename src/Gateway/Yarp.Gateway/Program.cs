using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Yarp.Gateway.Authentication;
using Yarp.Gateway.Authentication.MySSO.Configuration;
using Yarp.Gateway.Authentication.MySSO.Models;
using Yarp.Gateway.Authentication.MySSO.Options;
using Yarp.Gateway.Authentication.MySSO.Services;
using Yarp.Gateway.Authentication.Options;
using Yarp.Gateway.Configuration;
using Yarp.Gateway.Observability;
using Yarp.Gateway.YarpComponents;
using Yarp.Gateway.YarpComponents.TransformProviders;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

var machineName = Environment.GetEnvironmentVariable("MACHINENAME") ?? Environment.MachineName;

builder.Configuration.AddAuthenticationConfigurationJsons();

builder.Configuration.AddYarpConfigurationJsons();

var gatewayAuthConfiguration = GatewayAuthConfiguration.GatewayAuthSettingOptions(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
                         .AllowAnyHeader()
                         .AllowAnyMethod();

        // 讓前端（含跨網域 XHR/fetch）能讀取 session 到期時間 header。
        var sessionExpiresHeader = gatewayAuthConfiguration?.MySSO?.SessionExpiresHeaderName;
        if (!string.IsNullOrWhiteSpace(sessionExpiresHeader))
        {
            corsPolicyBuilder.WithExposedHeaders(sessionExpiresHeader);
        }
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost;
});

builder.Services.AddHealthChecks();

// 由於 Yarp 本身就有類似 HttpLogging 的功能，所以這邊不會使用 AddHttpLogging
// builder.Services.AddHttpLogging(
//     loggingOptions =>
//     {
//         loggingOptions.LoggingFields = HttpLoggingFields.All;
//     });

builder.Services.AddW3CLogging(logging =>
{
    // Log all W3C fields
    logging.LoggingFields = W3CLoggingFields.All;

    logging.FileSizeLimit = 5 * 1024 * 1024;
    logging.RetainedFileCountLimit = 2;

    logging.FileName = $"{machineName}_";
    logging.FlushInterval = TimeSpan.FromSeconds(2);

    logging.AdditionalRequestHeaders.Add("x-forwarded-for");
});

builder.Services.AddYarpAuthentication(gatewayAuthConfiguration);

builder.Services.AddAuthorizationBuilder()
       .AddPolicy("GatewayManager", policy =>
       {
           policy.RequireAuthenticatedUser();
           policy.RequireRole("Gateway-Administrator");
           // policy.RequireClaim("scope", "gateway-manager");
       });

builder.Services.AddHttpClient();
builder.Services.AddHttpForwarder();

// Add the reverse proxy capability to the server
builder.Services
       .AddReverseProxy()
       .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
       .AddTransforms<AuthenticationTokenTransformProvider>();

builder.Services.AddYarpMetrics();

builder.Services.AddOpenTelemetry()
       .WithLogging(loggerProviderBuilder =>
       {
           loggerProviderBuilder.AddOtlpExporter();
       })
       .WithTracing(tracerProviderBuilder =>
       {
           tracerProviderBuilder
               // 如果專案內有自訂追蹤資料的話，就需要將自訂的 source name 加進去
               .AddSource(ObservabilitySource.Name)
               .AddHttpClientInstrumentation()
               .AddAspNetCoreInstrumentation()
               // .AddEntityFrameworkCoreInstrumentation()
               .AddOtlpExporter(exporterOptions =>
               {
                   var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                   exporterOptions.Endpoint = !string.IsNullOrWhiteSpace(endpoint)
                                                  ? new Uri(endpoint)
                                                  : new Uri("http://localhost:4317");
                   exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
               });
       })
       .WithMetrics(meterProviderBuilder =>
       {
           meterProviderBuilder
               // 如果專案內有自訂 Metrics 資料的話，就需要將自訂的 source name 加進去
               .AddMeter(ObservabilitySource.Name)
               .AddRuntimeInstrumentation()
               .AddHttpClientInstrumentation()
               .AddAspNetCoreInstrumentation()
               .AddProcessInstrumentation()
               .AddOtlpExporter((exporterOptions, metricReaderOptions) =>
               {
                   var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                   exporterOptions.Endpoint = !string.IsNullOrWhiteSpace(endpoint)
                                                  ? new Uri(endpoint)
                                                  : new Uri("http://localhost:4317");
                   exporterOptions.ExportProcessorType = ExportProcessorType.Batch;

                   metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
               });
       });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto |
                       ForwardedHeaders.XForwardedHost
});

app.UseHealthChecks("/health");

// app.MapGet("/favicon.ico", () => "");
app.MapGet("/favicon.ico", () => Results.File(Path.Combine(AppContext.BaseDirectory, "wwwroot", "favicon.ico"), "images/ico"));

app.UseW3CLogging();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

if (gatewayAuthConfiguration?.MySSO is not null)
{
    var mySso = gatewayAuthConfiguration.MySSO;

    // 將 sliding 續期後的 session 到期時間，透過回應 header 回傳給前端校正倒數。
    app.Use(async (context, next) =>
    {
        context.Response.OnStarting(() =>
        {
            if (context.Items.TryGetValue(MySsoAuthenticationDefaults.SessionExpiresItemKey, out var value) &&
                value is DateTimeOffset expiresAt)
            {
                context.Response.Headers[mySso.SessionExpiresHeaderName] = expiresAt.ToString("O");
            }

            return Task.CompletedTask;
        });

        await next(context);
    });

    app.MapGet(
        mySso.LoginPath.Value!,
        (string? returnUrl) =>
        {
            var redirectUri = IsLocalReturnUrl(returnUrl)
                                  ? returnUrl!
                                  : "/";
            return Results.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = redirectUri
                },
                [MySsoAuthenticationDefaults.RemoteScheme]);
        });

    // 前端查詢登入狀態與 session 到期時間；同時觸發 sliding 續期並帶上到期 header。
    app.MapGet(
        mySso.SessionPath.Value!,
        async (HttpContext context) =>
        {
            var result = await context.AuthenticateAsync(MySsoAuthenticationDefaults.SessionScheme);
            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }

            var expiresAt =
                context.Items.TryGetValue(MySsoAuthenticationDefaults.SessionExpiresItemKey, out var value) &&
                value is DateTimeOffset itemExpiresAt
                    ? itemExpiresAt
                    : result.Properties?.ExpiresUtc;

            return Results.Ok(new
            {
                authenticated = true,
                expiresAt,
                idleTimeoutSeconds = (int)TimeSpan.FromMinutes(mySso.SessionIdleTimeoutMinutes).TotalSeconds
            });
        });

    // 前端主動以 refresh token 換發新平台權杖並延長 session；失敗則清除 session 由前端重新登入。
    app.MapPost(
        mySso.RefreshPath.Value!,
        async (
            HttpContext context,
            IMySsoTokenExchangeClient tokenExchangeClient,
            IOptionsMonitor<MySsoAuthenticationOptions> optionsMonitor) =>
        {
            var result = await context.AuthenticateAsync(MySsoAuthenticationDefaults.SessionScheme);
            if (!result.Succeeded || result.Principal is null || result.Properties is null)
            {
                await context.SignOutAsync(MySsoAuthenticationDefaults.SessionScheme);
                return Results.Unauthorized();
            }

            var refreshToken = result.Properties.GetTokenValue(OpenIdConnectParameterNames.RefreshToken);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                await context.SignOutAsync(MySsoAuthenticationDefaults.SessionScheme);
                return Results.Unauthorized();
            }

            var remoteOptions = optionsMonitor.Get(MySsoAuthenticationDefaults.RemoteScheme);

            MySsoTokenExchangeResult refreshResult;
            try
            {
                refreshResult = await tokenExchangeClient
                                      .RefreshAsync(refreshToken, remoteOptions, context.RequestAborted)
                                      .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception)
            {
                await context.SignOutAsync(MySsoAuthenticationDefaults.SessionScheme);
                return Results.Unauthorized();
            }

            if (!refreshResult.IsSuccess)
            {
                await context.SignOutAsync(MySsoAuthenticationDefaults.SessionScheme);
                return Results.Unauthorized();
            }

            // 以新權杖覆寫保存的 token，並清除既有到期資訊讓 cookie handler 重新計算到期時間以延長 session。
            refreshResult.StoreTokens(result.Properties);
            result.Properties.IssuedUtc = null;
            result.Properties.ExpiresUtc = null;
            await context.SignInAsync(MySsoAuthenticationDefaults.SessionScheme, result.Principal, result.Properties);

            if (result.Properties.ExpiresUtc is { } refreshedExpiresAt)
            {
                context.Items[MySsoAuthenticationDefaults.SessionExpiresItemKey] = refreshedExpiresAt;
            }

            return Results.Ok(new
            {
                expiresAt = result.Properties.ExpiresUtc
            });
        });
}

app.MapGet("/gateway-config",
           [Authorize("GatewayManager")] ([FromServices] IProxyConfigProvider proxyConfig) =>
           proxyConfig.GetConfig().ToGatewayConfig());

app.MapReverseProxy();

app.Run();

static bool IsLocalReturnUrl(string? returnUrl)
{
    return !string.IsNullOrWhiteSpace(returnUrl) &&
           returnUrl[0] == '/' &&
           (returnUrl.Length == 1 || returnUrl[1] is not '/' and not '\\');
}
