using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.UseCase.Dtos.Cluster;

/// <summary>
/// A cluster is a group of equivalent endpoints and associated policies.
/// </summary>
public class ClusterDto
{
    /// <summary>
    /// Cluster Id
    /// </summary>
    public string ClusterId { get; set; }

    /// <summary>
    /// 叢集說明
    /// </summary>
    public string? ClusterDescription { get; set; }

    /// <summary>
    /// The set of destinations associated with this cluster.
    /// </summary>
    public IReadOnlyDictionary<string, DestinationDto>? Destinations { get; set; }

    /// <summary>
    /// Load balancing policy.
    /// </summary>
    public LoadBalancingPolicies? LoadBalancingPolicy { get; set; }

    /// <summary>
    /// Session affinity config.
    /// </summary>
    public SessionAffinityDto? SessionAffinity { get; set; }

    /// <summary>
    /// Health checking config.
    /// </summary>
    public HealthCheckDto? HealthCheck { get; set; }

    /// <summary>
    /// Config for the HTTP client that is used to call destinations in this cluster.
    /// </summary>
    public HttpClientDto? HttpClient { get; set; }

    /// <summary>
    /// Config for outgoing HTTP requests.
    /// </summary>
    public HttpRequestDto? HttpRequest { get; set; }

    /// <summary>
    /// Arbitrary key-value pairs that further describe this cluster.
    /// </summary>
    public IReadOnlyDictionary<string, string>? MetaData { get; set; }
}