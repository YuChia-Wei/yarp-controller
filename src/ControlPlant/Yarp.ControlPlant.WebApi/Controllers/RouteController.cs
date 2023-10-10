using Mediator;
using Microsoft.AspNetCore.Mvc;
using Yarp.ControlPlant.UseCase.Commands.Route;
using Yarp.ControlPlant.UseCase.Queries;
using Yarp.ControlPlant.WebApi.Models.MappingExtension;
using Yarp.ControlPlant.WebApi.Models.Router;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp routing config api
/// </summary>
// [Authorize]
[Route("api/route")]
[ApiController]
public class RouteController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="mediator"></param>
    public RouteController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Adds a new route.
    /// </summary>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost]
    public async Task<IActionResult> AddRouteAsync(CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Globally unique identifier of the route.
    /// </summary>
    /// <param name="routeId"></param>
    /// <param name="router"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns></returns>
    [HttpPost("{routeId}")]
    public async Task<ActionResult> CreateRouteAsync([FromRoute] string routeId, [FromBody] Router router, CancellationToken cancellationToken)
    {
        var createClusterCommand = new CreateRoutingCommand
        {
            RouteId = routeId,
            Match = router.MatchRule.MapToDto(),
            Order = router.Order,
            ClusterId = router.ClusterId,
            AuthorizationPolicy = router.AuthorizationPolicy.ToString(),
            RateLimiterPolicy = router.RateLimiterPolicy,
            CorsPolicy = router.CorsPolicy,
            MaxRequestBodySize = router.MaxRequestBodySize,
            Metadata = router.Metadata,
            Transforms = router.Transforms
        };

        var executeResult = await this._mediator.Send(createClusterCommand);

        if (!executeResult.IsSuccess)
        {
            return this.BadRequest();
        }

        return this.Ok();
    }

    /// <summary>
    /// Removes a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpDelete("{routeId}")]
    public async Task<IActionResult> DeleteRouteAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the information of a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("{routeId}")]
    public async Task<IActionResult> GetRouteByIdAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// 取得指定叢集設定
    /// </summary>
    /// <param name="routerId"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns></returns>
    [HttpGet("{routerId}")]
    public async Task<ActionResult> GetRouterAsync([FromRoute] string routerId, CancellationToken cancellationToken)
    {
        var specifyRouteQuery = new SpecifyRouteQuery { RouterId = routerId };

        var queryResult = await this._mediator.Send(specifyRouteQuery);

        if (queryResult.IsFounded)
        {
            var queryResponse = queryResult.QueryResponse;

            Enum.TryParse<RouteAuthorizationPolicy>(queryResponse.AuthorizationPolicy, out var authPolicyResult);

            var cluster = new Router
            {
                RouterDescription = queryResponse.RouterDescription,
                MatchRule = queryResponse.MatchDto.MapToViewModel(),
                Order = queryResponse.Order,
                ClusterId = queryResponse.ClusterId,
                AuthorizationPolicy = authPolicyResult,
                RateLimiterPolicy = queryResponse.RateLimiterPolicy,
                CorsPolicy = queryResponse.CorsPolicy,
                MaxRequestBodySize = queryResponse.MaxRequestBodySize,
                Metadata = queryResponse.Metadata,
                Transforms = queryResponse.Transforms
            };

            return this.Ok(cluster);
        }

        return this.NoContent();
    }

    /// <summary>
    /// Retrieves the list of routes.
    /// </summary>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet]
    public async Task<IActionResult> GetRoutesAsync(CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Updates the information of a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPut("{routeId}")]
    public async Task<IActionResult> UpdateRouteAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}