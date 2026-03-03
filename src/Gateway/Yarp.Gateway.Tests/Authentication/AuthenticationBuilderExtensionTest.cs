using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Yarp.Gateway.Authentication;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Tests.Authentication;

public class AuthenticationBuilderExtensionTest
{
    private readonly ServiceCollection _serviceCollection;
    private GatewayAuthConfiguration? _gatewayAuthSettingOptions;

    public AuthenticationBuilderExtensionTest()
    {
        this._serviceCollection = new ServiceCollection();
    }

    [Fact]
    public void AddYarpAuthentication_GivenJwtOptions_ShouldSuccess()
    {
        this._gatewayAuthSettingOptions = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.Jwt,
            Jwt = new JwtAuthConfiguration
            {
                Authority = "",
                Audience = ""
            }
        };

        this._serviceCollection.AddYarpAuthentication(this._gatewayAuthSettingOptions);

        var buildServiceProvider = this._serviceCollection.BuildServiceProvider();

        var authenticationSchemeProvider = buildServiceProvider.GetService<IAuthenticationSchemeProvider>();

        Assert.NotNull(authenticationSchemeProvider);
    }

    [Fact]
    public void AddYarpAuthentication_GivenNullOptions_ThrowException()
    {
        this._gatewayAuthSettingOptions = null;

        Assert.Throws<ArgumentNullException>(() => this._serviceCollection.AddYarpAuthentication(this._gatewayAuthSettingOptions));
    }
}