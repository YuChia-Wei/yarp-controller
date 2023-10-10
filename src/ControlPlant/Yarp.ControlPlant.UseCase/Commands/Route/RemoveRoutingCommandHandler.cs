using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Port.Input;

namespace Yarp.ControlPlant.UseCase.Commands.Route;

public class RemoveRoutingCommandHandler : IRequestHandler<RemoveRoutingCommand, ExecuteResult>
{
    public ValueTask<ExecuteResult> Handle(RemoveRoutingCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}