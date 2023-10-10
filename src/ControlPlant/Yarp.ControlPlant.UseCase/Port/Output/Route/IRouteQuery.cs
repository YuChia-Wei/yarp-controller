using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Port.Output.Route;

public interface IRouteQuery
{
    Task<RouteConfig?> QueryAsync(string routeId);
}