using Microsoft.AspNetCore.Http;

namespace Yarp.ControlPlant.UseCase.Dtos.Cluster;

/// <summary>
/// Config for session affinity cookies.
/// </summary>
public class CookieDto
{
    /// <summary>
    /// The cookie path.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// The domain to associate the cookie with.
    /// </summary>
    public string? Domain { get; set; }

    /// <summary>
    /// Indicates whether a cookie is accessible by client-side script.
    /// </summary>
    /// <remarks>Defaults to "true".</remarks>
    public bool? HttpOnly { get; set; }

    /// <summary>
    /// The policy that will be used to determine <see cref="CookieOptions.Secure" />.
    /// </summary>
    /// <remarks>Defaults to <see cref="CookieSecurePolicy.None" />.</remarks>
    public CookieSecurePolicy? SecurePolicy { get; set; }

    /// <summary>
    /// The SameSite attribute of the cookie.
    /// </summary>
    /// <remarks>Defaults to <see cref="SameSiteMode.Unspecified" />.</remarks>
    public SameSiteMode? SameSite { get; set; }

    /// <summary>
    /// Gets or sets the lifespan of a cookie.
    /// </summary>
    public TimeSpan? Expiration { get; set; }

    /// <summary>
    /// Gets or sets the max-age for the cookie.
    /// </summary>
    public TimeSpan? MaxAge { get; set; }

    /// <summary>
    /// Indicates if this cookie is essential for the application to function correctly. If true then
    /// consent policy checks may be bypassed.
    /// </summary>
    /// <remarks>Defaults to "false".</remarks>
    public bool? IsEssential { get; set; }
}