using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using StackExchange.Redis;
using Yarp.Gateway.Authentication.Jwt;
using Yarp.Gateway.Authentication.OpenIdConnect;
using Yarp.Gateway.Authentication.Options;
using Yarp.Gateway.Configuration;
using Yarp.Gateway.Observability;

var builder = WebApplication.CreateBuilder(args);

var gatewayName = Environment.GetEnvironmentVariable("GatewayName") ?? AppDomain.CurrentDomain.FriendlyName;
var machineName = Environment.GetEnvironmentVariable("MACHINENAME") ?? Environment.MachineName;

builder.Configuration.AddAuthenticationConfigurations();
builder.Configuration.AddYarpConfigurations();

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

    logging.FileName = $"{gatewayName}_{machineName}_";
    logging.FlushInterval = TimeSpan.FromSeconds(2);

    logging.AdditionalRequestHeaders.Add("x-forwarded-for");
});

// Cookie OAuth 會需要設定 HA Server 時的外部資料儲存來源
// 在 Cookie Auth 區塊有設定 SessionStore 的狀況下，這個設定無效 (via. https://github.com/dotnet/AspNetCore.Docs/issues/21163 )
// ref: https://learn.microsoft.com/en-us/aspnet/core/security/cookie-sharing?view=aspnetcore-7.0#share-authentication-cookies-with-aspnet-core-identity
// TODO: Cookie Auth 的儲存做法還需要再評估
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

builder.Services
       .AddDataProtection()
       .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnectionString),
                                        $"{gatewayName}:LoginCookies:")
       .SetApplicationName(gatewayName);

// TODO: Auth 相關: Default Scheme 目前使用 OpenIdConnect + Cookie，未來如何動態化設定需要討論
// TODO: Auth 相關: 目前 Auth 設定必須在容器部署時就進行設定 (因為需要使用 appsetting 中的資料)，如何動態處理需要再研究作法
builder.Services.AddAuthentication(options =>
       {
           options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
           options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
       })
       .AddOpenIdConnectWithCookie(builder.Configuration.CreateOpidAuthOptions(), redisConnectionString)
       .AddJwtAuthentication(builder.Configuration.CreateJwtAuthOptions());

builder.Services.AddHttpForwarder();

// Add the reverse proxy capability to the server
builder.Services
       .AddReverseProxy()
       .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

app.UseW3CLogging();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseHealthChecks("/health");

// TODO: 攔截 favicon.ico 的路徑的做法還需要研究，這邊的作法並非最佳做法且無效
// app.MapGet("/favicon.ico", () => "");
// app.MapGet("/favicon.ico", () => Results.File(Path.Combine(AppContext.BaseDirectory,"wwwroot","favicon.ico"), "images/ico"));

app.UseAuthentication();

app.UseAuthorization();

app.MapReverseProxy();

app.Run();