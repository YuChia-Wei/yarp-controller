using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Yarp.Gateway.Authentication.Options;

public class OpidAuthOptions
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
    public IEnumerable<string> WebApiAudience { get; set; }

    /// <summary>
    /// Cookies 的名稱
    /// </summary>
    public string LoginCookieName { get; set; }

    /// <summary>
    /// Cookies 所屬網域
    /// </summary>
    public string LoginCookieDomain { get; set; }

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
    public string TicketStoreRedisServer { get; set; }

    /// <summary>
    /// 登入的服務名稱
    /// </summary>
    public string LoginApplicationName { get; set; }

    /// <summary>
    /// 是否有設定
    /// </summary>
    public bool IsSettled { get; set; } = true;

    public bool RequireHttpsMetadata { get; set; } = true;

    /// <summary>
    /// opid response type
    /// </summary>
    /// <remarks>code id_token</remarks>
    public string ResponseType { get; set; } = OpenIdConnectResponseType.CodeIdToken;

    public string? RefreshTokenAddress { get; set; }
}