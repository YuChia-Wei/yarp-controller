using Yarp.ControlPlant.UseCase.Dtos;
using Yarp.ControlPlant.UseCase.Dtos.MappingExtension;
using Yarp.ControlPlant.UseCase.Dtos.Router;
using Yarp.ControlPlant.UseCase.Port.Input;
using Yarp.ControlPlant.UseCase.Port.Output.Route;

namespace Yarp.ControlPlant.UseCase.Queries;

public class SpecifyRouteQueryHandler : IQueryHandler<SpecifyRouteQuery, QueryResult<RouterDto>>
{
    private readonly IRouteQuery _routeQuery;

    public SpecifyRouteQueryHandler(IRouteQuery routeQuery)
    {
        this._routeQuery = routeQuery;
    }

    public async ValueTask<QueryResult<RouterDto>> Handle(SpecifyRouteQuery query, CancellationToken cancellationToken)
    {
        var router = await this._routeQuery.QueryAsync(query.RouterId);

        if (router is null)
        {
            return QueryResult<RouterDto>.NotFound();
        }

        var routerDto = new RouterDto()
        {
            RouteId = router.RouteId,
            MatchDto = router.Match.MapToDto(),
            Order = router.Order,
            ClusterId = router.ClusterId,
            AuthorizationPolicy = router.AuthorizationPolicy,
            RateLimiterPolicy = router.RateLimiterPolicy,
            CorsPolicy = router.CorsPolicy,
            MaxRequestBodySize = router.MaxRequestBodySize,
            Metadata = router.Metadata,
            Transforms = router.Transforms
        };
        return QueryResult<RouterDto>.Found(routerDto);
    }
}