using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Exceptions;
using Yarp.ControlPlant.UseCase.Port.Input;
using Yarp.ControlPlant.UseCase.Port.Input.Route;
using Yarp.ControlPlant.UseCase.Port.Output.Route;
using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Commands.Route;

public class CreateRoutingCommandHandler : IRequestHandler<CreateRoutingCommand, ExecuteResult>
{
    private readonly IRouteCreator _routeCreator;
    private readonly IRouteExistChecker _routeExistChecker;

    public CreateRoutingCommandHandler(IRouteCreator routeCreator, IRouteExistChecker routeExistChecker)
    {
        this._routeCreator = routeCreator;
        this._routeExistChecker = routeExistChecker;
    }

    public async ValueTask<ExecuteResult> Handle(CreateRoutingCommand request, CancellationToken cancellationToken)
    {
        var isExist = await this._routeExistChecker.CheckExistAsync(request.RouteId);

        if (isExist)
        {
            throw new RouterDuplicateException(request.RouteId);
        }

        var routeConfig = new RouteConfig
        {
            RouteId = request.RouteId,
            Match = request.Match.MapToEntity(),
            Order = request.Order,
            ClusterId = request.ClusterId,
            AuthorizationPolicy = request.AuthorizationPolicy,
            RateLimiterPolicy = request.RateLimiterPolicy,
            CorsPolicy = request.CorsPolicy,
            MaxRequestBodySize = request.MaxRequestBodySize,
            Metadata = request.Metadata,
            Transforms = request.Transforms
        };

        await this._routeCreator.CreateAsync(routeConfig);

        return ExecuteResult<string>.Success(request.RouteId);
    }
}