using System.Text.Json.Serialization;

namespace Yarp.Gateway.Authentication.Options;

public class GatewayAuthConfiguration
{
    public const string JsonSectionName = "GatewayAuthSetting";

    [JsonPropertyName("Default")]
    public DefaultAuthMethod Default { get; init; } = DefaultAuthMethod.Opid;

    [JsonPropertyName("Jwt")]
    public JwtAuthConfiguration? Jwt { get; init; }

    [JsonPropertyName("Opid")]
    public OpidAuthConfiguration? Opid { get; init; }

    /// <summary>
    /// MySSO form-post remote authentication 設定。
    /// </summary>
    [JsonPropertyName("MySSO")]
    public MySsoAuthenticationConfiguration? MySSO { get; init; }

    /// <summary>
    /// 每次 request 都會呼叫外部服務驗證 token 或 key 的設定。
    /// </summary>
    [JsonPropertyName("ExternalToken")]
    public ExternalTokenAuthenticationConfiguration? ExternalToken { get; init; }

    public static GatewayAuthConfiguration? GatewayAuthSettingOptions(ConfigurationManager builderConfiguration)
    {
        return builderConfiguration.GetSection(JsonSectionName).Get<GatewayAuthConfiguration>();
    }
}