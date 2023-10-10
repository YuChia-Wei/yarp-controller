using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Port.Input;

namespace Yarp.ControlPlant.UseCase.Commands.Cluster;

public class ApplyGatewayClusterConfigCommandHandler : IRequestHandler<ApplyGatewayClusterConfigCommand, ExecuteResult>
{
    public ValueTask<ExecuteResult> Handle(ApplyGatewayClusterConfigCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}