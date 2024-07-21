using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Yarp.Gateway.Authentication.Options;

public class OpidAuthConfiguration
{
    /// <summary>
    /// Authority Url
    /// </summary>
    public required string Authority { get; set; }

    /// <summary>
    /// ClientId
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    /// <value>The client secret.</value>
    public required string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    /// <value>The audience.</value>
    public required IEnumerable<string> WebApiAudience { get; set; }

    /// <summary>
    /// Cookies 的名稱
    /// </summary>
    public required string LoginCookieName { get; set; }

    /// <summary>
    /// Cookies 所屬網域
    /// </summary>
    public required string LoginCookieDomain { get; set; }

    /// <summary>
    /// Cookie Secure Policy (default is non)
    /// </summary>
    public CookieSecurePolicy CookieSecurePolicy { get; set; } = CookieSecurePolicy.None;

    /// <summary>
    /// Cookie Same Site Mode (default is Lex)
    /// </summary>
    public SameSiteMode CookieSameSiteMode { get; set; } = SameSiteMode.Lax;

    /// <summary>
    /// ticket store redis server url
    /// </summary>
    public required string TicketStoreRedisServer { get; set; }

    /// <summary>
    /// 登入的服務名稱
    /// </summary>
    public required string LoginApplicationName { get; set; }

    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// opid response type
    /// </summary>
    /// <remarks>code id_token</remarks>
    public string ResponseType { get; set; } = OpenIdConnectResponseType.CodeIdToken;

    public string? RefreshTokenAddress { get; set; }
}