using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Port.Output.Route;

public interface IRouteCreator
{
    Task CreateAsync(RouteConfig routeConfig);
}