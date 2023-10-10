// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Describes a destination of a cluster.
/// </summary>
public class DestinationConfig
{
    /// <summary>
    /// Address of this destination. E.g. <c>https://127.0.0.1:123/abcd1234/</c>.
    /// This field is required.
    /// </summary>
    public string Address { get; set; } = default!;

    /// <summary>
    /// Endpoint accepting active health check probes. E.g. <c>http://127.0.0.1:1234/</c>.
    /// </summary>
    public string? Health { get; set; }

    /// <summary>
    /// Arbitrary key-value pairs that further describe this destination.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }

    public bool Equals(DestinationConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(this.Address, other.Address, StringComparison.OrdinalIgnoreCase)
               && string.Equals(this.Health, other.Health, StringComparison.OrdinalIgnoreCase)
               && CaseSensitiveEqualHelper.Equals(this.Metadata, other.Metadata);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Address?.GetHashCode(StringComparison.OrdinalIgnoreCase), this.Health?.GetHashCode(StringComparison.OrdinalIgnoreCase),
                                CaseSensitiveEqualHelper.GetHashCode(this.Metadata));
    }
}