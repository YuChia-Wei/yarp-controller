// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Session affinity options.
/// </summary>
public class SessionAffinityConfig
{
    /// <summary>
    /// Indicates whether session affinity is enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// The session affinity policy to use.
    /// </summary>
    public string? Policy { get; set; }

    /// <summary>
    /// Strategy handling missing destination for an affinitized request.
    /// </summary>
    public string? FailurePolicy { get; set; }

    /// <summary>
    /// Identifies the name of the field where the affinity value is stored.
    /// For the cookie affinity policy this will be the cookie name.
    /// For the header affinity policy this will be the header name.
    /// The policy will give its own default if no value is set.
    /// This value should be unique across clusters to avoid affinity conflicts.
    /// https://github.com/microsoft/reverse-proxy/issues/976
    /// This field is required.
    /// </summary>
    public string AffinityKeyName { get; set; } = default!;

    /// <summary>
    /// Configuration of a cookie storing the session affinity key in case
    /// the <see cref="Policy" /> is set to 'Cookie'.
    /// </summary>
    public SessionAffinityCookieConfig? Cookie { get; set; }

    public bool Equals(SessionAffinityConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Enabled == other.Enabled
               && string.Equals(this.Policy, other.Policy, StringComparison.OrdinalIgnoreCase)
               && string.Equals(this.FailurePolicy, other.FailurePolicy, StringComparison.OrdinalIgnoreCase)
               && string.Equals(this.AffinityKeyName, other.AffinityKeyName, StringComparison.Ordinal)
               && this.Cookie == other.Cookie;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Enabled, this.Policy?.GetHashCode(StringComparison.OrdinalIgnoreCase),
                                this.FailurePolicy?.GetHashCode(StringComparison.OrdinalIgnoreCase),
                                this.AffinityKeyName?.GetHashCode(StringComparison.Ordinal), this.Cookie);
    }
}