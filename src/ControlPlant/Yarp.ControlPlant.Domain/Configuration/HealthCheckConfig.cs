// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// All health check config.
/// </summary>
public class HealthCheckConfig
{
    /// <summary>
    /// Passive health check config.
    /// </summary>
    public PassiveHealthCheckConfig? Passive { get; set; }

    /// <summary>
    /// Active health check config.
    /// </summary>
    public ActiveHealthCheckConfig? Active { get; set; }

    /// <summary>
    /// Available destinations policy.
    /// </summary>
    public string? AvailableDestinationsPolicy { get; set; }

    public bool Equals(HealthCheckConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Passive == other.Passive
               && this.Active == other.Active
               && string.Equals(this.AvailableDestinationsPolicy, other.AvailableDestinationsPolicy, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Passive, this.Active, this.AvailableDestinationsPolicy?.GetHashCode(StringComparison.OrdinalIgnoreCase));
    }
}