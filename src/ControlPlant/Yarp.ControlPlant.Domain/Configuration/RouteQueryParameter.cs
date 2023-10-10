// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Route criteria for a query parameter that must be present on the incoming request.
/// </summary>
public class RouteQueryParameter
{
    /// <summary>
    /// Name of the query parameter to look for.
    /// This field is case insensitive and required.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// A collection of acceptable query parameter values used during routing.
    /// </summary>
    public IReadOnlyList<string>? Values { get; set; }

    /// <summary>
    /// Specifies how query parameter values should be compared (e.g. exact matches Vs. contains).
    /// Defaults to <see cref="QueryParameterMatchMode.Exact" />.
    /// </summary>
    public QueryParameterMatchMode Mode { get; set; }

    /// <summary>
    /// Specifies whether query parameter value comparisons should ignore case.
    /// When <c>true</c>, <see cref="StringComparison.Ordinal" /> is used.
    /// When <c>false</c>, <see cref="StringComparison.OrdinalIgnoreCase" /> is used.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool IsCaseSensitive { get; set; }

    public bool Equals(RouteQueryParameter? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase)
               && this.Mode == other.Mode
               && this.IsCaseSensitive == other.IsCaseSensitive
               && (this.IsCaseSensitive
                       ? CaseSensitiveEqualHelper.Equals(this.Values, other.Values)
                       : CaseInsensitiveEqualHelper.Equals(this.Values, other.Values));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Name?.GetHashCode(StringComparison.OrdinalIgnoreCase), this.Mode, this.IsCaseSensitive, this.IsCaseSensitive
                                    ? CaseSensitiveEqualHelper.GetHashCode(this.Values)
                                    : CaseInsensitiveEqualHelper.GetHashCode(this.Values));
    }
}