using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Yarp.Gateway.Authentication;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
                         .AllowAnyHeader()
                         .AllowAnyMethod();
    });
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

builder.Services.AddYarpAuthentication(GatewayAuthConfiguration.GatewayAuthSettingOptions(builder.Configuration));

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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost;
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

app.MapGet("/gateway-config",
           [Authorize("GatewayManager")]([FromServices] IProxyConfigProvider proxyConfig) =>
           proxyConfig.GetConfig().ToGatewayConfig());

app.MapReverseProxy();

app.Run();