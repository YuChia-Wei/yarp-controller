using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Xunit;
using Yarp.Gateway.Authentication.Options;

namespace Yarp.Gateway.Tests;

public class ConfigurationTests
{
    [Fact]
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
        Assert.NotNull(appSettings);
        Assert.Equal(DefaultAuthMethod.Jwt, appSettings.Default);
        Assert.Null(appSettings.Opid);
        Assert.NotNull(appSettings.Jwt);
    }

    [Fact]
    public void ConfigurationSection_GetAuthConfiguration_Opid_Success()
    {
        // Arrange
        var jsonConfig = """
                         {
                           "GatewayAuthSetting": {
                             "Default": "Opid",
                             "Opid": {
                               "ClientId": "test_client",
                               "ClientSecret": "<CLIENT_SECRET_PLACEHOLDER>",
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
        Assert.NotNull(appSettings);
        Assert.Equal(DefaultAuthMethod.Opid, appSettings.Default);
        Assert.NotNull(appSettings.Opid);
        Assert.Null(appSettings.Jwt);
    }
}