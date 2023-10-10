using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Port.Input;

namespace Yarp.ControlPlant.UseCase.Commands.Cluster;

public class ModifyClusterCommandHandler : IRequestHandler<ModifyClusterCommand, ExecuteResult>
{
    public ValueTask<ExecuteResult> Handle(ModifyClusterCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}