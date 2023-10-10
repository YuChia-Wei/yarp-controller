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
    /// Redis Ticket Store Server Url
    /// </summary>
    public string CookieTicketStoreRedisServerUrl { get; set; } = string.Empty;

    /// <summary>
    /// Cookies 的名稱
    /// </summary>
    public string CookieLoginName { get; set; }

    /// <summary>
    /// Cookies 所屬網域
    /// </summary>
    public string CookieLoginDomain { get; set; }

    /// <summary>
    /// Cookie Secure Policy (default is non)
    /// </summary>
    public CookieSecurePolicy CookieSecurePolicy { get; set; } = CookieSecurePolicy.None;

    /// <summary>
    /// Cookie Same Site Mode (default is Lex)
    /// </summary>
    public SameSiteMode CookieSameSiteMode { get; set; } = SameSiteMode.Lax;
}