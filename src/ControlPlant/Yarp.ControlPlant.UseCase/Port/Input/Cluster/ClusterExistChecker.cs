namespace Yarp.ControlPlant.UseCase.Port.Input.Cluster;

public class ClusterExistChecker : IClusterExistChecker
{
    public async Task<bool> CheckExistAsync(string clusterId)
    {
        return true;
    }
}