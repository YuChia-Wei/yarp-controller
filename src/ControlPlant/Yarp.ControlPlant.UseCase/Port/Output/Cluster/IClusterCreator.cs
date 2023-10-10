using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Port.Output.Cluster;

public interface IClusterCreator
{
    Task CreateAsync(ClusterConfig clusterConfig);
}