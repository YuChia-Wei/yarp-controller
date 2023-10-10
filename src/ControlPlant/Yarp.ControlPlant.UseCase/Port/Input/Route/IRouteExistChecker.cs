namespace Yarp.ControlPlant.UseCase.Port.Input.Route;

public interface IRouteExistChecker
{
    Task<bool> CheckExistAsync(string clusterId);
}