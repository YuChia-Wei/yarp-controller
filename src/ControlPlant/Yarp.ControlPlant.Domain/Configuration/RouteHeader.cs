// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Route criteria for a header that must be present on the incoming request.
/// </summary>
public class RouteHeader
{
    /// <summary>
    /// Name of the header to look for.
    /// This field is case insensitive and required.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// A collection of acceptable header values used during routing. Only one value must match.
    /// The list must not be empty unless using <see cref="HeaderMatchMode.Exists" /> or
    /// <see cref="HeaderMatchMode.NotExists" />.
    /// </summary>
    public IReadOnlyList<string>? Values { get; set; }

    /// <summary>
    /// Specifies how header values should be compared (e.g. exact matches Vs. by prefix).
    /// Defaults to <see cref="HeaderMatchMode.ExactHeader" />.
    /// </summary>
    public HeaderMatchMode Mode { get; set; }

    /// <summary>
    /// Specifies whether header value comparisons should ignore case.
    /// When <c>true</c>, <see cref="StringComparison.Ordinal" /> is used.
    /// When <c>false</c>, <see cref="StringComparison.OrdinalIgnoreCase" /> is used.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool IsCaseSensitive { get; set; }

    public bool Equals(RouteHeader? other)
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