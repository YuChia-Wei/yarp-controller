// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Passive health check config.
/// </summary>
public class PassiveHealthCheckConfig
{
    /// <summary>
    /// Whether passive health checks are enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Passive health check policy.
    /// </summary>
    public string? Policy { get; set; }

    /// <summary>
    /// Destination reactivation period after which an unhealthy destination is considered healthy again.
    /// </summary>
    public TimeSpan? ReactivationPeriod { get; set; }

    public bool Equals(PassiveHealthCheckConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Enabled == other.Enabled
               && string.Equals(this.Policy, other.Policy, StringComparison.OrdinalIgnoreCase)
               && this.ReactivationPeriod == other.ReactivationPeriod;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Enabled, this.Policy?.GetHashCode(StringComparison.OrdinalIgnoreCase), this.ReactivationPeriod);
    }
}