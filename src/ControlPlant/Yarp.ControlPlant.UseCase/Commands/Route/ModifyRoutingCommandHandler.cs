using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Port.Input;

namespace Yarp.ControlPlant.UseCase.Commands.Route;

public class ModifyRoutingCommandHandler : IRequestHandler<ModifyRoutingCommand, ExecuteResult>
{
    public ValueTask<ExecuteResult> Handle(ModifyRoutingCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}