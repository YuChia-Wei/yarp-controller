using System.Text.Json.Serialization;

namespace Yarp.Gateway.Authentication.Options;

public class GatewayAuthSettingOptions
{
    public DefaultAuthEnum Default { get; set; } = DefaultAuthEnum.Opid;

    [JsonPropertyName("Jwt")]
    public JwtAuthOptions? Jwt { get; set; }

    [JsonPropertyName("Opid")]
    public OpidAuthOptions? Opid { get; set; }
}