using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Dtos.Cluster;
using Yarp.ControlPlant.UseCase.Dtos.MappingExtension;
using Yarp.ControlPlant.UseCase.Port.Input;
using Yarp.ControlPlant.UseCase.Port.Output.Cluster;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.UseCase.Queries;

public class SpecifyClusterQueryHandler : IQueryHandler<SpecifyClusterQuery, QueryResult<ClusterDto>>
{
    private readonly IClusterQuery _clusterQuery;

    public SpecifyClusterQueryHandler(IClusterQuery clusterQuery)
    {
        this._clusterQuery = clusterQuery;
    }

    public async ValueTask<QueryResult<ClusterDto>> Handle(SpecifyClusterQuery query, CancellationToken cancellationToken)
    {
        var cluster = await this._clusterQuery.QueryAsync(query.ClusterId);
        if (cluster is null)
        {
            return QueryResult<ClusterDto>.NotFound();
        }

        var tryParse = Enum.TryParse<LoadBalancingPolicies>(cluster.LoadBalancingPolicy, out var result);
        var clusterDto = new ClusterDto
        {
            ClusterId = cluster.ClusterId,
            ClusterDescription = cluster.Description,
            LoadBalancingPolicy = tryParse ? result : null,
            SessionAffinity = cluster.SessionAffinity?.MapToDto(),
            HealthCheck = cluster.HealthCheck?.MapToDto(),
            HttpClient = cluster.HttpClient?.MapToDto(),
            HttpRequest = cluster.HttpRequest?.MapToDto(),
            Destinations = cluster.Destinations?.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value.MapToDto()),
            MetaData = cluster.Metadata
        };
        return QueryResult<ClusterDto>.Found(clusterDto);
    }
}