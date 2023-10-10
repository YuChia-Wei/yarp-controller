using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp gateway sync api
/// </summary>
// [Authorize]
[Route("api/gateway")]
[ApiController]
public class GatewaysRouteController : ControllerBase
{
    private readonly IMediator _mediator;

    public GatewaysRouteController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// 將指定路由設定加入至指定 gateway
    /// </summary>
    /// <param name="gatewayId"></param>
    /// <param name="routeId"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns></returns>
    [HttpPost("{gatewayId}/route/{routeId}")]
    public async Task<ActionResult> AddRouteSettingToGatewayAsync([FromRoute] string gatewayId, [FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Applies route configurations to the specified gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{gatewayId}/route/Apply")]
    public async Task<IActionResult> ApplyRouteConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the online route configurations of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("{gatewayId}/route/Online")]
    public async Task<IActionResult> GetOnlineRouteConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}