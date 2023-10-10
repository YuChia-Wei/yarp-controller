using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Exceptions;
using Yarp.ControlPlant.UseCase.Port.Input;
using Yarp.ControlPlant.UseCase.Port.Input.Cluster;
using Yarp.ControlPlant.UseCase.Port.Output.Cluster;
using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Commands.Cluster;

public class OverrideClusterCommandHandler : IRequestHandler<OverrideClusterCommand, ExecuteResult>
{
    private readonly IClusterCreator _clusterCreator;
    private readonly IClusterExistChecker _clusterExistChecker;

    public OverrideClusterCommandHandler(IClusterCreator clusterCreator, IClusterExistChecker clusterExistChecker)
    {
        this._clusterCreator = clusterCreator;
        this._clusterExistChecker = clusterExistChecker;
    }

    public async ValueTask<ExecuteResult> Handle(OverrideClusterCommand request, CancellationToken cancellationToken)
    {
        var isExist = await this._clusterExistChecker.CheckExistAsync(request.ClusterId);

        if (isExist)
        {
            throw new ClusterDuplicateException(request.ClusterId);
        }

        var clusterConfig = new ClusterConfig
        {
            ClusterId = request.ClusterId,
            Description = request.ClusterDescription,
            LoadBalancingPolicy = request.LoadBalancingPolicy.ToString(),
            SessionAffinity = request.SessionAffinity?.MapToEntity(),
            HealthCheck = request.HealthCheck?.MapToEntity(),
            HttpClient = request.HttpClient?.MapToEntity(),
            HttpRequest = request.HttpRequest?.MapToEntity(),
            Destinations = request.Destinations?.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value.MapToEntity()),
            Metadata = request.Metadata
        };

        await this._clusterCreator.CreateAsync(clusterConfig);

        return ExecuteResult<string>.Success(request.ClusterId);
    }
}