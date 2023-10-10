// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Forwarder;
using Yarp.ReverseProxy.ControlPlant.Entity.Utilities;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// A cluster is a group of equivalent endpoints and associated policies.
/// </summary>
public class ClusterConfig
{
    /// <summary>
    /// The Id for this cluster. This needs to be globally unique.
    /// This field is required.
    /// </summary>
    public string ClusterId { get; set; } = default!;

    /// <summary>
    /// 叢集說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Load balancing policy.
    /// </summary>
    public string? LoadBalancingPolicy { get; set; }

    /// <summary>
    /// Session affinity config.
    /// </summary>
    public SessionAffinityConfig? SessionAffinity { get; set; }

    /// <summary>
    /// Health checking config.
    /// </summary>
    public HealthCheckConfig? HealthCheck { get; set; }

    /// <summary>
    /// Config for the HTTP client that is used to call destinations in this cluster.
    /// </summary>
    public HttpClientConfig? HttpClient { get; set; }

    /// <summary>
    /// Config for outgoing HTTP requests.
    /// </summary>
    public ForwarderRequestConfig? HttpRequest { get; set; }

    /// <summary>
    /// The set of destinations associated with this cluster.
    /// </summary>
    public IReadOnlyDictionary<string, DestinationConfig>? Destinations { get; set; }

    /// <summary>
    /// Arbitrary key-value pairs that further describe this cluster.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }

    public bool Equals(ClusterConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.EqualsExcludingDestinations(other)
               && CollectionEqualityHelper.Equals(this.Destinations, other.Destinations);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.ClusterId?.GetHashCode(StringComparison.OrdinalIgnoreCase),
                                this.LoadBalancingPolicy?.GetHashCode(StringComparison.OrdinalIgnoreCase), this.SessionAffinity, this.HealthCheck,
                                this.HttpClient, this.HttpRequest,
                                CollectionEqualityHelper.GetHashCode(this.Destinations),
                                CaseSensitiveEqualHelper.GetHashCode(this.Metadata));
    }

    internal bool EqualsExcludingDestinations(ClusterConfig other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(this.ClusterId, other.ClusterId, StringComparison.OrdinalIgnoreCase)
               && string.Equals(this.LoadBalancingPolicy, other.LoadBalancingPolicy, StringComparison.OrdinalIgnoreCase)
               // CS0252 warning only shows up in VS https://github.com/dotnet/roslyn/issues/49302
               && this.SessionAffinity == other.SessionAffinity
               && this.HealthCheck == other.HealthCheck
               && this.HttpClient == other.HttpClient
               && this.HttpRequest == other.HttpRequest
               && CaseSensitiveEqualHelper.Equals(this.Metadata, other.Metadata);
    }
}