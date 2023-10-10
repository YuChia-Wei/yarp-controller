namespace Yarp.ControlPlant.UseCase.Exceptions;

public class RouterDuplicateException : Exception
{
    public RouterDuplicateException(string routeId)
        : base($"路由 {routeId} 已存在")
    {
    }
}