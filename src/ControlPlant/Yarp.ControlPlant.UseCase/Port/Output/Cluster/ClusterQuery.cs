using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Port.Output.Cluster;

public class ClusterQuery : IClusterQuery
{
    public Task<ClusterConfig?> QueryAsync(string clusterId)
    {
        throw new NotImplementedException();
    }
}