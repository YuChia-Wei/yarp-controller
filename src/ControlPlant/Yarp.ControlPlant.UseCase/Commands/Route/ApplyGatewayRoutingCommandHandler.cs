using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Port.Input;

namespace Yarp.ControlPlant.UseCase.Commands.Route;

public class ApplyGatewayRoutingCommandHandler : IRequestHandler<ApplyGatewayRoutingCommand, ExecuteResult>
{
    public ValueTask<ExecuteResult> Handle(ApplyGatewayRoutingCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}