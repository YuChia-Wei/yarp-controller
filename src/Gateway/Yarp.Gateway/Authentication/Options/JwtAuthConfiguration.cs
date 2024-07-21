namespace Yarp.Gateway.Authentication.Options;

/// <summary>
/// OAuth 設定物件
/// </summary>
public class JwtAuthConfiguration
{
    /// <summary>
    /// Authority Url
    /// </summary>
    public required string Authority { get; set; }

    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    /// <value>The audience.</value>
    public required string Audience { get; set; }

    /// <summary>
    /// 需要 HttpsMetadata
    /// </summary>
    public bool RequireHttpsMetadata { get; set; }
}