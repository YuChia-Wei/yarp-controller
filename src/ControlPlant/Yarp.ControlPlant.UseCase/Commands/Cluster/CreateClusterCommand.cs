using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Dtos.Cluster;
using Yarp.ControlPlant.UseCase.Port.Input;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.UseCase.Commands.Cluster;

/// <summary>
/// 建立新叢集
/// </summary>
public class CreateClusterCommand : IRequest<ExecuteResult>
{
    /// <summary>
    /// The Id for this cluster. This needs to be globally unique.
    /// This field is required.
    /// </summary>
    public string ClusterId { get; set; } = default!;

    /// <summary>
    /// 叢集說明
    /// </summary>
    public string? ClusterDescription { get; set; } = default!;

    /// <summary>
    /// Load balancing policy.
    /// </summary>
    public LoadBalancingPolicies? LoadBalancingPolicy { get; set; }

    /// <summary>
    /// Session affinity Dto.
    /// </summary>
    public SessionAffinityDto? SessionAffinity { get; set; }

    /// <summary>
    /// Health checking Dto.
    /// </summary>
    public HealthCheckDto? HealthCheck { get; set; }

    /// <summary>
    /// Dto for the HTTP client that is used to call destinations in this cluster.
    /// </summary>
    public HttpClientDto? HttpClient { get; set; }

    /// <summary>
    /// Dto for outgoing HTTP requests.
    /// </summary>
    public HttpRequestDto? HttpRequest { get; set; }

    /// <summary>
    /// The set of destinations associated with this cluster.
    /// </summary>
    public IReadOnlyDictionary<string, DestinationDto>? Destinations { get; set; }

    /// <summary>
    /// Arbitrary key-value pairs that further describe this cluster.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }
}