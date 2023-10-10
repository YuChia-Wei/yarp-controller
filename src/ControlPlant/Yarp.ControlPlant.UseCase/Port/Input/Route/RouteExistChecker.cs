namespace Yarp.ControlPlant.UseCase.Port.Input.Route;

public class RouteExistChecker : IRouteExistChecker
{
    public async Task<bool> CheckExistAsync(string clusterId)
    {
        return false;
    }
}