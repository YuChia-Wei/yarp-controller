namespace Yarp.ControlPlant.UseCase.Exceptions;

public class ClusterDuplicateException : Exception
{
    public ClusterDuplicateException(string clusterId)
        : base($"叢集 {clusterId} 已存在")
    {
    }
}