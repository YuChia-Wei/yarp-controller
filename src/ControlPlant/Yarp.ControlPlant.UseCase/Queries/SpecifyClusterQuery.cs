using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Dtos.Cluster;
using Yarp.ControlPlant.UseCase.Port.Input;

namespace Yarp.ControlPlant.UseCase.Queries;

public class SpecifyClusterQuery : IQuery<QueryResult<ClusterDto>>
{
    public string ClusterId { get; set; }
}