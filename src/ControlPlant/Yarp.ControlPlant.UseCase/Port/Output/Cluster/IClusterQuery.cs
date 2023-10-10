using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Port.Output.Cluster;

public interface IClusterQuery
{
    Task<ClusterConfig?> QueryAsync(string clusterId);
}