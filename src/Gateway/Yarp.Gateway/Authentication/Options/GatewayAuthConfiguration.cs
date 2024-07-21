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
}