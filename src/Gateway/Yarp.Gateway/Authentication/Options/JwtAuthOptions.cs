namespace Yarp.Gateway.Authentication.Options;

/// <summary>
/// OAuth 設定物件
/// </summary>
public class JwtAuthOptions
{
    /// <summary>
    /// Authority Url
    /// </summary>
    public string Authority { get; set; }

    /// <summary>
    /// ClientId
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>The client secret.</value>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    /// <value>The audience.</value>
    public string Audience { get; set; }

    /// <summary>
    /// 需要 HttpsMetadata
    /// </summary>
    public bool RequireHttpsMetadata { get; set; }
}