using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using Yarp.Gateway.Authentication;
using Yarp.Gateway.Authentication.ExternalToken.Configuration;
using Yarp.Gateway.Authentication.MySSO.Configuration;
using Yarp.Gateway.Authentication.MySSO.Options;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Tests.Authentication;

public class AuthenticationBuilderExtensionTest
{
    private readonly ServiceCollection _serviceCollection = new();

    [Fact]
    public void AddYarpAuthentication_GivenJwtOptions_ShouldSuccess()
    {
        var options = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.Jwt,
            Jwt = new JwtAuthConfiguration
            {
                Authority = "",
                Audience = ""
            }
        };

        this._serviceCollection.AddYarpAuthentication(options);

        using var serviceProvider = this._serviceCollection.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IAuthenticationSchemeProvider>());
    }

    [Fact]
    public void ExternalTokenAuthenticationConfiguration_GivenDefaults_ShouldNotAcceptBearerFallback()
    {
        var options = new ExternalTokenAuthenticationConfiguration();

        Assert.Equal(ExternalTokenAuthenticationDefaults.AuthenticationScheme, options.TokenScheme);
        Assert.Equal(ExternalTokenAuthenticationDefaults.KeyScheme, options.KeyScheme);
        Assert.False(options.AcceptBearerToken);
        Assert.True(options.IgnoreJwtBearerToken);
    }

    [Fact]
    public void MySsoAuthenticationConfiguration_GivenDefaults_ShouldUseDedicatedSessionCookie()
    {
        var options = new MySsoAuthenticationConfiguration();

        Assert.Equal(MySsoAuthenticationDefaults.CallbackPath, options.CallbackPath);
        Assert.Equal(MySsoAuthenticationDefaults.LoginPath, options.LoginPath);
        Assert.Equal(MySsoAuthenticationDefaults.SessionCookieName, options.SessionCookieName);
        Assert.Equal(30, options.SessionIdleTimeoutMinutes);
        Assert.False(options.AllowQueryStringCallback);
        Assert.Equal(MySsoAuthenticationDefaults.SessionPath, options.SessionPath);
        Assert.Equal(MySsoAuthenticationDefaults.RefreshPath, options.RefreshPath);
        Assert.Equal(MySsoSessionRenewalMode.Periodic, options.SessionRenewalMode);
        Assert.Equal(60, options.SessionRenewalIntervalSeconds);
        Assert.Equal(MySsoAuthenticationDefaults.SessionExpiresHeaderName, options.SessionExpiresHeaderName);
    }

    [Fact]
    public void AddYarpAuthentication_GivenMySsoWithoutTicketStoreRedis_ThrowException()
    {
        var options = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.MySSO,
            MySSO = new MySsoAuthenticationConfiguration
            {
                AuthorizationEndpoint = new Uri("https://mysso.example.test/login"),
                TokenExchangeEndpoint = new Uri("https://platform-auth.example.test/mysso/exchange")
            }
        };

        Assert.Throws<InvalidOperationException>(() => this._serviceCollection.AddYarpAuthentication(options));
    }

    [Fact]
    public async Task AddYarpAuthentication_GivenMySsoOptions_ShouldUseSessionWithoutDefaultRemoteChallenge()
    {
        var options = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.MySSO,
            MySSO = CreateMySsoConfiguration()
        };

        this._serviceCollection.AddYarpAuthentication(options);

        using var serviceProvider = this._serviceCollection.BuildServiceProvider();
        var schemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();

        Assert.Equal(
            MySsoAuthenticationDefaults.SessionScheme,
            (await schemeProvider.GetDefaultAuthenticateSchemeAsync())?.Name);
        Assert.Equal(
            MySsoAuthenticationDefaults.SessionScheme,
            (await schemeProvider.GetDefaultChallengeSchemeAsync())?.Name);
        Assert.NotNull(await schemeProvider.GetSchemeAsync(MySsoAuthenticationDefaults.RemoteScheme));
        Assert.NotNull(await schemeProvider.GetSchemeAsync(MySsoAuthenticationDefaults.SessionScheme));
        Assert.NotNull(await schemeProvider.GetSchemeAsync(MySsoAuthenticationDefaults.InteractiveScheme));

        var authorizationOptions = serviceProvider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;
        var interactivePolicy = authorizationOptions.GetPolicy(MySsoAuthenticationDefaults.InteractivePolicy);

        Assert.NotNull(interactivePolicy);
        Assert.Contains(MySsoAuthenticationDefaults.InteractiveScheme, interactivePolicy.AuthenticationSchemes);

        var policySchemeOptions = serviceProvider
                                  .GetRequiredService<IOptionsMonitor<PolicySchemeOptions>>()
                                  .Get(MySsoAuthenticationDefaults.InteractiveScheme);
        Assert.Equal(MySsoAuthenticationDefaults.SessionScheme, policySchemeOptions.ForwardAuthenticate);
        Assert.Equal(MySsoAuthenticationDefaults.RemoteScheme, policySchemeOptions.ForwardChallenge);

        var cookieOptions = serviceProvider
                            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                            .Get(MySsoAuthenticationDefaults.SessionScheme);
        var httpContext = new DefaultHttpContext();
        var redirectContext = new RedirectContext<CookieAuthenticationOptions>(
            httpContext,
            new AuthenticationScheme(
                MySsoAuthenticationDefaults.SessionScheme,
                null,
                typeof(CookieAuthenticationHandler)),
            cookieOptions,
            new AuthenticationProperties(),
            "/account/login");

        await cookieOptions.Events.RedirectToLogin(redirectContext);

        Assert.Equal(StatusCodes.Status401Unauthorized, httpContext.Response.StatusCode);
        Assert.Contains(
            httpContext.Response.Headers.SetCookie,
            value => value?.StartsWith($"{MySsoAuthenticationDefaults.SessionCookieName}=", StringComparison.Ordinal) == true);

        var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<MySsoAuthenticationOptions>>();
        var remoteOptions = optionsMonitor.Get(MySsoAuthenticationDefaults.RemoteScheme);

        Assert.NotNull(remoteOptions.StateDataFormat);
        Assert.Equal(new Uri("https://mysso.example.test/login"), remoteOptions.AuthorizationEndpoint);
        Assert.Equal(new Uri("https://platform-auth.example.test/mysso/exchange"), remoteOptions.TokenExchangeEndpoint);
    }

    [Fact]
    public async Task AddYarpAuthentication_GivenExternalTokenOptions_ShouldRegisterExternalScheme()
    {
        var options = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.ExternalToken,
            ExternalToken = CreateExternalTokenConfiguration()
        };

        this._serviceCollection.AddYarpAuthentication(options);

        using var serviceProvider = this._serviceCollection.BuildServiceProvider();
        var schemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();

        Assert.Equal(
            ExternalTokenAuthenticationDefaults.AuthenticationScheme,
            (await schemeProvider.GetDefaultAuthenticateSchemeAsync())?.Name);
        Assert.NotNull(await schemeProvider.GetSchemeAsync(ExternalTokenAuthenticationDefaults.AuthenticationScheme));
    }

    [Fact]
    public async Task AddYarpAuthentication_GivenJwtMySsoAndExternalToken_ShouldUsePolicyScheme()
    {
        var options = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.MySSO,
            Jwt = new JwtAuthConfiguration
            {
                Authority = "",
                Audience = ""
            },
            MySSO = CreateMySsoConfiguration(),
            ExternalToken = CreateExternalTokenConfiguration()
        };

        this._serviceCollection.AddYarpAuthentication(options);

        using var serviceProvider = this._serviceCollection.BuildServiceProvider();
        var schemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();

        Assert.Equal(
            MySsoAuthenticationDefaults.PolicyScheme,
            (await schemeProvider.GetDefaultAuthenticateSchemeAsync())?.Name);
        Assert.Equal(
            MySsoAuthenticationDefaults.PolicyScheme,
            (await schemeProvider.GetDefaultChallengeSchemeAsync())?.Name);
        Assert.NotNull(await schemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme));
        Assert.NotNull(await schemeProvider.GetSchemeAsync(MySsoAuthenticationDefaults.RemoteScheme));
        Assert.NotNull(await schemeProvider.GetSchemeAsync(MySsoAuthenticationDefaults.SessionScheme));
        Assert.NotNull(await schemeProvider.GetSchemeAsync(MySsoAuthenticationDefaults.InteractiveScheme));
        Assert.NotNull(await schemeProvider.GetSchemeAsync(ExternalTokenAuthenticationDefaults.AuthenticationScheme));
        Assert.NotNull(await schemeProvider.GetSchemeAsync(MySsoAuthenticationDefaults.PolicyScheme));
    }

    [Fact]
    public void AddYarpAuthentication_GivenNullOptions_ThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => this._serviceCollection.AddYarpAuthentication(null));
    }

    private static ExternalTokenAuthenticationConfiguration CreateExternalTokenConfiguration()
    {
        return new ExternalTokenAuthenticationConfiguration
        {
            TokenValidationEndpoint = new Uri("https://external-auth.example.test/token/check"),
            KeyExchangeEndpoint = new Uri("https://external-auth.example.test/key/exchange")
        };
    }

    private static MySsoAuthenticationConfiguration CreateMySsoConfiguration()
    {
        return new MySsoAuthenticationConfiguration
        {
            AuthorizationEndpoint = new Uri("https://mysso.example.test/login"),
            TokenExchangeEndpoint = new Uri("https://platform-auth.example.test/mysso/exchange"),
            RefreshTokenEndpoint = new Uri("https://platform-auth.example.test/mysso/refresh"),
            TicketStoreRedisServer = "localhost:6379"
        };
    }
}