using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Port.Input;

namespace Yarp.ControlPlant.UseCase.Commands.Cluster;

public class RemoveClusterCommandHandler : IRequestHandler<RemoveClusterCommand, ExecuteResult>
{
    public ValueTask<ExecuteResult> Handle(RemoveClusterCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}