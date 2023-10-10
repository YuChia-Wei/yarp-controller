using Mediator;
using Microsoft.AspNetCore.Mvc;
using Yarp.ControlPlant.UseCase.Commands.Cluster;
using Yarp.ControlPlant.UseCase.Queries;
using Yarp.ControlPlant.WebApi.Models.Cluster;
using Yarp.ControlPlant.WebApi.Models.MappingExtension;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp cluster config api
/// </summary>
// [Authorize]
[Route("api/cluster")]
[ApiController]
public class ClusterController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="mediator"></param>
    public ClusterController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// 建立新的叢集設定
    /// </summary>
    /// <param name="clusterId"></param>
    /// <param name="cluster"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns></returns>
    [HttpPost("{clusterId}")]
    public async Task<ActionResult> CreateClusterAsync([FromRoute] string clusterId, [FromBody] Cluster cluster, CancellationToken cancellationToken)
    {
        var createClusterCommand = new CreateClusterCommand
        {
            ClusterId = clusterId,
            ClusterDescription = cluster.ClusterDescription,
            LoadBalancingPolicy = cluster.LoadBalancingPolicy,
            SessionAffinity = cluster.SessionAffinity?.MapToDto(),
            HealthCheck = cluster.HealthCheck?.MapToDto(),
            HttpClient = cluster.HttpClient?.MapToDto(),
            HttpRequest = cluster.HttpRequest?.MapToDto(),
            Destinations = cluster.Destinations?.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value.MapToDto()),
            Metadata = cluster.MetaData
        };

        var executeResult = await this._mediator.Send(createClusterCommand, cancellationToken);

        if (!executeResult.IsSuccess)
        {
            return this.BadRequest();
        }

        return this.Ok();
    }

    /// <summary>
    /// 取得指定叢集設定
    /// </summary>
    /// <param name="clusterId"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns></returns>
    [HttpGet("{clusterId}")]
    public async Task<ActionResult> GetClusterAsync([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        var getClusterCommand = new SpecifyClusterQuery { ClusterId = clusterId };

        var clusterConfig = await this._mediator.Send(getClusterCommand, cancellationToken);

        if (clusterConfig.IsFounded)
        {
            var clusterConfigQueryResponse = clusterConfig.QueryResponse;

            var cluster = new Cluster
            {
                ClusterDescription = clusterConfigQueryResponse.ClusterDescription,
                Destinations =
                    clusterConfigQueryResponse.Destinations?.ToDictionary(keyValuePair => keyValuePair.Key,
                                                                          keyValuePair => keyValuePair.Value.MapToViewModel()),
                LoadBalancingPolicy = clusterConfigQueryResponse.LoadBalancingPolicy,
                SessionAffinity = clusterConfigQueryResponse.SessionAffinity?.MapToViewModel(),
                HealthCheck = clusterConfigQueryResponse.HealthCheck?.MapToViewModel(),
                HttpClient = clusterConfigQueryResponse.HttpClient?.MapToViewModel(),
                HttpRequest = clusterConfigQueryResponse.HttpRequest?.MapToViewModel(),
                MetaData = clusterConfigQueryResponse.MetaData
            };

            return this.Ok(cluster);
        }

        return this.Ok();
    }

    /// <summary>
    /// Gets the cluster by its ID.
    /// </summary>
    [HttpGet("{clusterId}")]
    public async Task<IActionResult> GetClusterById([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Gets the list of clusters.
    /// </summary>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet]
    public async Task<IActionResult> GetClusters(CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the HTTP client information of the cluster.
    /// </summary>
    [HttpGet("{clusterId}/httpClient")]
    public async Task<IActionResult> GetHttpClient([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the HTTP request information of the cluster.
    /// </summary>
    [HttpGet("{clusterId}/httpRequest")]
    public async Task<IActionResult> GetHttpRequest([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the load balance information of the cluster.
    /// </summary>
    [HttpGet("{clusterId}/loadBalance")]
    public async Task<IActionResult> GetLoadBalance([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the session information of the cluster.
    /// </summary>
    [HttpGet("{clusterId}/session")]
    public async Task<IActionResult> GetSession([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Checks the health of the cluster.
    /// </summary>
    [HttpGet("{clusterId}/healthCheck")]
    public async Task<IActionResult> HealthCheck([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Overrides the settings of the cluster.
    /// </summary>
    [HttpPost("{clusterId}/override")]
    public async Task<IActionResult> OverrideCluster([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// 覆蓋現有叢集
    /// </summary>
    /// <param name="clusterId"></param>
    /// <param name="cluster"></param>
    /// <returns></returns>
    [HttpPut("{clusterId}")]
    public async Task<ActionResult> OverrideClusterAsync([FromRoute] string clusterId, [FromBody] Cluster cluster)
    {
        var createClusterCommand = new OverrideClusterCommand
        {
            ClusterId = clusterId,
            ClusterDescription = cluster.ClusterDescription,
            LoadBalancingPolicy = cluster.LoadBalancingPolicy,
            SessionAffinity = cluster.SessionAffinity?.MapToDto(),
            HealthCheck = cluster.HealthCheck?.MapToDto(),
            HttpClient = cluster.HttpClient?.MapToDto(),
            HttpRequest = cluster.HttpRequest?.MapToDto(),
            Destinations = cluster.Destinations?.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value.MapToDto()),
            Metadata = cluster.MetaData
        };

        var executeResult = await this._mediator.Send(createClusterCommand);

        if (!executeResult.IsSuccess)
        {
            return this.BadRequest();
        }

        return this.Ok();
    }
}