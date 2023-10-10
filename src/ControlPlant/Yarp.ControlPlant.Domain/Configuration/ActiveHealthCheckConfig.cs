// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Active health check config.
/// </summary>
public class ActiveHealthCheckConfig
{
    /// <summary>
    /// Whether active health checks are enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Health probe interval.
    /// </summary>
    public TimeSpan? Interval { get; set; }

    /// <summary>
    /// Health probe timeout, after which a destination is considered unhealthy.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Active health check policy.
    /// </summary>
    public string? Policy { get; set; }

    /// <summary>
    /// HTTP health check endpoint path.
    /// </summary>
    public string? Path { get; set; }

    public bool Equals(ActiveHealthCheckConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Enabled == other.Enabled
               && this.Interval == other.Interval
               && this.Timeout == other.Timeout
               && string.Equals(this.Policy, other.Policy, StringComparison.OrdinalIgnoreCase)
               && string.Equals(this.Path, other.Path, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Enabled, this.Interval, this.Timeout, this.Policy?.GetHashCode(StringComparison.OrdinalIgnoreCase),
                                this.Path?.GetHashCode(StringComparison.Ordinal));
    }
}