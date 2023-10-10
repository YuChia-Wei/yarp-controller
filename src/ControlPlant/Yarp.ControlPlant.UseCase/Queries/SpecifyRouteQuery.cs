using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Dtos.Router;
using Yarp.ControlPlant.UseCase.Port.Input;

namespace Yarp.ControlPlant.UseCase.Queries;

public class SpecifyRouteQuery : IQuery<QueryResult<RouterDto>>
{
    public string RouterId { get; set; }
}