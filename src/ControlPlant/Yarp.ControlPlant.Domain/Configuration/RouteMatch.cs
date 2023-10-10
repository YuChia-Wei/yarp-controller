// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Describes the matching criteria for a route.
/// </summary>
public class RouteMatch
{
    /// <summary>
    /// Only match requests that use these optional HTTP methods. E.g. GET, POST.
    /// </summary>
    public IReadOnlyList<string>? Methods { get; set; }

    /// <summary>
    /// Only match requests with the given Host header.
    /// Supports wildcards and ports. For unicode host names, do not use punycode.
    /// </summary>
    public IReadOnlyList<string>? Hosts { get; set; }

    /// <summary>
    /// Only match requests with the given Path pattern.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Only match requests that contain all of these query parameters.
    /// </summary>
    public IReadOnlyList<RouteQueryParameter>? QueryParameters { get; set; }

    /// <summary>
    /// Only match requests that contain all of these headers.
    /// </summary>
    public IReadOnlyList<RouteHeader>? Headers { get; set; }

    public bool Equals(RouteMatch? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(this.Path, other.Path, StringComparison.OrdinalIgnoreCase)
               && CaseInsensitiveEqualHelper.Equals(this.Hosts, other.Hosts)
               && CaseInsensitiveEqualHelper.Equals(this.Methods, other.Methods)
               && CollectionEqualityHelper.Equals(this.Headers, other.Headers)
               && CollectionEqualityHelper.Equals(this.QueryParameters, other.QueryParameters);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Path?.GetHashCode(StringComparison.OrdinalIgnoreCase),
                                CaseInsensitiveEqualHelper.GetHashCode(this.Hosts),
                                CaseInsensitiveEqualHelper.GetHashCode(this.Methods),
                                CollectionEqualityHelper.GetHashCode(this.Headers),
                                CollectionEqualityHelper.GetHashCode(this.QueryParameters));
    }
}