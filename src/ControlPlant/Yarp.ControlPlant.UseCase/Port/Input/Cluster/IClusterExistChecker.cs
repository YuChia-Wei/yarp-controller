namespace Yarp.ControlPlant.UseCase.Port.Input.Cluster;

public interface IClusterExistChecker
{
    Task<bool> CheckExistAsync(string clusterId);
}