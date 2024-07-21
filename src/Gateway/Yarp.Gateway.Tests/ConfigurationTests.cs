using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Tests;

[TestFixture]
public class ConfigurationTests
{
    [Test]
    public void ConfigurationSection_GetAuthConfiguration_Jwt_Success()
    {
        // Arrange
        var jsonConfig = """
                         {
                           "GatewayAuthSetting": {
                             "Default": "Jwt",
                             "Jwt": {
                                 "Authority": "http://localhost:8080",
                                 "Audience": "",
                                 "RequireHttpsMetadata": true
                             }
                           }
                         }
                         """;

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonConfig));
        var configuration = new ConfigurationBuilder()
                            .AddJsonStream(memoryStream)
                            .Build();

        // Act
        var appSettings = configuration.GetSection(GatewayAuthConfiguration.JsonSectionName).Get<GatewayAuthConfiguration>();

        // Assert
        appSettings!.Default.Should().Be(DefaultAuthMethod.Jwt);
        appSettings!.Opid.Should().BeNull();
        appSettings!.Jwt.Should().NotBeNull();
    }

    [Test]
    public void ConfigurationSection_GetAuthConfiguration_Opid_Success()
    {
        // Arrange
        var jsonConfig = """
                         {
                           "GatewayAuthSetting": {
                             "Default": "Opid",
                             "Opid": {
                               "ClientId": "test_client",
                               "ClientSecret": "6ZyOGaalxmk0NIZCg9w81lIU9bxnDL4P",
                               "Authority": "http://localhost:8080/realms/master/",
                               "WebApiAudience": [
                                 "profile",
                                 "roles"
                               ],
                               "LoginApplicationName": "my_app",
                               "LoginCookieName": "local_login",
                               "LoginCookieDomain": "",
                               "TicketStoreRedisServer": "localhost:6379",
                               "RequireHttpsMetadata": false,
                               "ResponseType": "code",
                               "RefreshTokenAddress": "http://localhost:8080/realms/master/protocol/openid-connect/token"
                             }
                           }
                         }
                         """;

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonConfig));
        var configuration = new ConfigurationBuilder()
                            .AddJsonStream(memoryStream)
                            .Build();

        // Act
        var appSettings = configuration.GetSection(GatewayAuthConfiguration.JsonSectionName).Get<GatewayAuthConfiguration>();

        // Assert
        appSettings!.Default.Should().Be(DefaultAuthMethod.Opid);
        appSettings!.Opid.Should().NotBeNull();
        appSettings!.Jwt.Should().BeNull();
    }
}