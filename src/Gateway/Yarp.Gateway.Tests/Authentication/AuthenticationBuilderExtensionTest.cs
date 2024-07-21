using System;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Yarp.Gateway.Authentication;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Tests.Authentication;

[TestFixture]
[TestOf(typeof(AuthenticationBuilderExtension))]
public class AuthenticationBuilderExtensionTest
{
    private ServiceCollection _serviceCollection;
    private GatewayAuthConfiguration? _gatewayAuthSettingOptions;

    [SetUp]
    public void SetUp()
    {
        this._serviceCollection = new ServiceCollection();
    }

    [Test]
    public void AddYarpAuthentication_GivenJwtOptions_ShouldSuccess()
    {
        this._gatewayAuthSettingOptions = new GatewayAuthConfiguration
        {
            Default = DefaultAuthMethod.Jwt, Jwt = new JwtAuthConfiguration { Authority = "", Audience = "" }
        };

        this._serviceCollection.AddYarpAuthentication(this._gatewayAuthSettingOptions);

        var buildServiceProvider = this._serviceCollection.BuildServiceProvider();

        var authenticationSchemeProvider = buildServiceProvider.GetService<IAuthenticationSchemeProvider>();

        authenticationSchemeProvider.Should().NotBeNull();
    }

    [Test]
    public void AddYarpAuthentication_GivenNullOptions_ThrowException()
    {
        this._gatewayAuthSettingOptions = null;

        var func = () => this._serviceCollection.AddYarpAuthentication(this._gatewayAuthSettingOptions);

        func.Should().Throw<ArgumentNullException>();
    }
}