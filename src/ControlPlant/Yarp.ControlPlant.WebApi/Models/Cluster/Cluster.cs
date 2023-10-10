using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.WebApi.Models.Cluster;

/// <summary>
/// A cluster is a group of equivalent endpoints and associated policies.
/// </summary>
public class Cluster
{
    /// <summary>
    /// 叢集說明
    /// </summary>
    public string? ClusterDescription { get; set; }

    /// <summary>
    /// The set of destinations associated with this cluster.
    /// </summary>
    public IReadOnlyDictionary<string, Destination>? Destinations { get; set; }

    /// <summary>
    /// Load balancing policy.
    /// </summary>
    public LoadBalancingPolicies? LoadBalancingPolicy { get; set; }

    /// <summary>
    /// Session affinity config.
    /// </summary>
    public SessionAffinity? SessionAffinity { get; set; }

    /// <summary>
    /// Health checking config.
    /// </summary>
    public HealthCheck? HealthCheck { get; set; }

    /// <summary>
    /// Config for the HTTP client that is used to call destinations in this cluster.
    /// </summary>
    public HttpClientSetting? HttpClient { get; set; }

    /// <summary>
    /// Config for outgoing HTTP requests.
    /// </summary>
    public HttpRequestSetting? HttpRequest { get; set; }

    /// <summary>
    /// Arbitrary key-value pairs that further describe this cluster.
    /// </summary>
    public IReadOnlyDictionary<string, string>? MetaData { get; set; }
}