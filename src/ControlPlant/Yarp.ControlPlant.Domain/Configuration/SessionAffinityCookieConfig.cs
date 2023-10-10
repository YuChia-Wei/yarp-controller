// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

// Mirrors CookieBuilder and CookieOptions
// https://github.com/dotnet/aspnetcore/blob/main/src/Http/Http.Abstractions/src/CookieBuilder.cs
/// <summary>
/// Config for session affinity cookies.
/// </summary>
public class SessionAffinityCookieConfig
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

    public bool Equals(SessionAffinityCookieConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(this.Path, other.Path, StringComparison.Ordinal)
               && string.Equals(this.Domain, other.Domain, StringComparison.OrdinalIgnoreCase)
               && this.HttpOnly == other.HttpOnly
               && this.SecurePolicy == other.SecurePolicy
               && this.SameSite == other.SameSite
               && this.Expiration == other.Expiration
               && this.MaxAge == other.MaxAge
               && this.IsEssential == other.IsEssential;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Path?.GetHashCode(StringComparison.Ordinal), this.Domain?.GetHashCode(StringComparison.OrdinalIgnoreCase), this.HttpOnly,
                                this.SecurePolicy, this.SameSite, this.Expiration, this.MaxAge, this.IsEssential);
    }
}