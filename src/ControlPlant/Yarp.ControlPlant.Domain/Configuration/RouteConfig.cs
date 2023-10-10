// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Describes a route that matches incoming requests based on the <see cref="Match" /> criteria
/// and proxies matching requests to the cluster identified by its <see cref="ClusterId" />.
/// </summary>
public class RouteConfig
{
    /// <summary>
    /// Globally unique identifier of the route.
    /// This field is required.
    /// </summary>
    public string RouteId { get; set; } = default!;

    /// <summary>
    /// Parameters used to match requests.
    /// This field is required.
    /// </summary>
    public RouteMatch Match { get; set; } = default!;

    /// <summary>
    /// Optionally, an order value for this route. Routes with lower numbers take precedence over higher numbers.
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// Gets or sets the cluster that requests matching this route
    /// should be proxied to.
    /// </summary>
    public string? ClusterId { get; set; }

    /// <summary>
    /// The name of the AuthorizationPolicy to apply to this route.
    /// If not set then only the FallbackPolicy will apply.
    /// Set to "Default" to enable authorization with the applications default policy.
    /// Set to "Anonymous" to disable all authorization checks for this route.
    /// </summary>
    public string? AuthorizationPolicy { get; set; }
#if NET7_0_OR_GREATER
    /// <summary>
    /// The name of the RateLimiterPolicy to apply to this route.
    /// If not set then only the GlobalLimiter will apply.
    /// Set to "Disable" to disable rate limiting for this route.
    /// Set to "Default" or leave empty to use the global rate limits, if any.
    /// </summary>
    public string? RateLimiterPolicy { get; set; }
#endif
    /// <summary>
    /// The name of the CorsPolicy to apply to this route.
    /// If not set then the route won't be automatically matched for cors preflight requests.
    /// Set to "Default" to enable cors with the default policy.
    /// Set to "Disable" to refuses cors requests for this route.
    /// </summary>
    public string? CorsPolicy { get; set; }

    /// <summary>
    /// An optional override for how large request bodies can be in bytes. If set, this overrides the server's default (30MB)
    /// per request.
    /// Set to '-1' to disable the limit for this route.
    /// </summary>
    public long? MaxRequestBodySize { get; set; }

    /// <summary>
    /// Arbitrary key-value pairs that further describe this route.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Parameters used to transform the request and response. See <see cref="ImmutableArray{T}.Builder.ITransformBuilder" />.
    /// </summary>
    public IReadOnlyList<IReadOnlyDictionary<string, string>>? Transforms { get; set; }

    public bool Equals(RouteConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Order == other.Order
               && string.Equals(this.RouteId, other.RouteId, StringComparison.OrdinalIgnoreCase)
               && string.Equals(this.ClusterId, other.ClusterId, StringComparison.OrdinalIgnoreCase)
               && string.Equals(this.AuthorizationPolicy, other.AuthorizationPolicy, StringComparison.OrdinalIgnoreCase)
#if NET7_0_OR_GREATER
               && string.Equals(this.RateLimiterPolicy, other.RateLimiterPolicy, StringComparison.OrdinalIgnoreCase)
#endif
               && string.Equals(this.CorsPolicy, other.CorsPolicy, StringComparison.OrdinalIgnoreCase)
               && this.Match == other.Match
               && CaseSensitiveEqualHelper.Equals(this.Metadata, other.Metadata)
               && CaseSensitiveEqualHelper.Equals(this.Transforms, other.Transforms);
    }

    public override int GetHashCode()
    {
        // HashCode.Combine(...) takes only 8 arguments
        var hash = new HashCode();
        hash.Add(this.Order);
        hash.Add(this.RouteId?.GetHashCode(StringComparison.OrdinalIgnoreCase));
        hash.Add(this.ClusterId?.GetHashCode(StringComparison.OrdinalIgnoreCase));
        hash.Add(this.AuthorizationPolicy?.GetHashCode(StringComparison.OrdinalIgnoreCase));
#if NET7_0_OR_GREATER
        hash.Add(this.RateLimiterPolicy?.GetHashCode(StringComparison.OrdinalIgnoreCase));
#endif
        hash.Add(this.CorsPolicy?.GetHashCode(StringComparison.OrdinalIgnoreCase));
        hash.Add(this.Match);
        hash.Add(CaseSensitiveEqualHelper.GetHashCode(this.Metadata));
        hash.Add(CaseSensitiveEqualHelper.GetHashCode(this.Transforms));
        return hash.ToHashCode();
    }
}