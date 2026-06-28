using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Yarp.Gateway.Authentication;
using Yarp.Gateway.Authentication.ExternalToken.Services;
using Yarp.Gateway.Authentication.MySSO;
using Yarp.Gateway.Authentication.MySSO.Configuration;
using Yarp.Gateway.Authentication.MySSO.Services;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Tests.FunctionalTests.Support;

internal sealed class GatewayTestApplication(WebApplication application, HttpClient client) : IAsyncDisposable
{
    public HttpClient Client { get; } = client;

    public static async Task<GatewayTestApplication> StartMySsoAsync(FakeMySsoTokenExchangeClient tokenExchangeClient)
    {
        var mySso = new MySsoAuthenticationConfiguration
        {
            AuthorizationEndpoint = new Uri("https://mysso.test/login"),
            TokenExchangeEndpoint = new Uri("https://platform-auth.test/mysso/exchange"),
            RefreshTokenEndpoint = new Uri("https://platform-auth.test/mysso/refresh"),
            AppId = "gateway-tests",
            TicketStoreRedisServer = "unused:6379",
            SessionCookieSecurePolicy = CookieSecurePolicy.Always,
            SessionIdleTimeoutMinutes = 30,
            SessionRenewalIntervalSeconds = 60
        };

        var gatewayAuth = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.MySSO,
            MySSO = mySso
        };

        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                EnvironmentName = "FunctionalTests"
            });
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Services.AddAuthorization();
        builder.Services.AddDataProtection()
               .UseEphemeralDataProtectionProvider();
        builder.Services.AddSingleton<IMySsoTokenExchangeClient>(tokenExchangeClient);
        builder.Services.AddYarpAuthentication(gatewayAuth);
        builder.Services.PostConfigure<CookieAuthenticationOptions>(
            MySsoAuthenticationDefaults.SessionScheme,
            options => options.SessionStore = new InMemoryTicketStore());

        var application = builder.Build();
        application.UseRouting();
        application.UseAuthentication();
        application.UseAuthorization();
        application.UseMySsoAuthentication(mySso);

        application.MapGet(
                       "/protected",
                       async (HttpContext context) =>
                       {
                           var accessToken = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                           return Results.Ok(new
                           {
                               customerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                               accessToken
                           });
                       })
                   .RequireAuthorization();

        application.MapGet("/interactive", () => Results.Ok())
                   .RequireAuthorization(MySsoAuthenticationDefaults.InteractivePolicy);

        await application.StartAsync();

        var client = application.GetTestClient();
        client.BaseAddress = new Uri("https://gateway.test");
        return new GatewayTestApplication(application, client);
    }

    public static async Task<GatewayTestApplication> StartExternalTokenAsync(
        FakeExternalTokenAuthenticationClient authenticationClient)
    {
        var gatewayAuth = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.ExternalToken,
            ExternalToken = new ExternalTokenAuthenticationConfiguration
            {
                TokenValidationEndpoint = new Uri("https://external-auth.test/token/check"),
                KeyExchangeEndpoint = new Uri("https://external-auth.test/key/exchange"),
                AcceptBearerToken = true,
                IgnoreJwtBearerToken = true
            }
        };

        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                EnvironmentName = "FunctionalTests"
            });
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IExternalTokenAuthenticationClient>(authenticationClient);
        builder.Services.AddYarpAuthentication(gatewayAuth);

        var application = builder.Build();
        application.UseRouting();
        application.UseAuthentication();
        application.UseAuthorization();
        application.MapGet(
                       "/external-protected",
                       (HttpContext context) => Results.Ok(new
                       {
                           userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       }))
                   .RequireAuthorization();

        await application.StartAsync();

        var client = application.GetTestClient();
        client.BaseAddress = new Uri("https://gateway.test");
        return new GatewayTestApplication(application, client);
    }

    public async ValueTask DisposeAsync()
    {
        this.Client.Dispose();
        await application.DisposeAsync();
    }
}