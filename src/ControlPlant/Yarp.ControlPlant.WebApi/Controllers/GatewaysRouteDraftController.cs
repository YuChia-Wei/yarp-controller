using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp gateway sync api
/// </summary>
// [Authorize]
[Route("api/gateway")]
[ApiController]
public class GatewaysRouteDraftController : ControllerBase
{
    private readonly IMediator _mediator;

    public GatewaysRouteDraftController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Appends draft route configurations to the specified gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{gatewayId}/route/draft/Append")]
    public async Task<IActionResult> AppendDraftRouteConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the draft route configurations of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("{gatewayId}/route/draft/Get")]
    public async Task<IActionResult> GetDraftRouteConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Overrides the draft route configurations of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{gatewayId}/route/draft/Override")]
    public async Task<IActionResult> OverrideDraftRouteConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Removes the draft route configurations of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpDelete("{gatewayId}/route/draft/Remove")]
    public async Task<IActionResult> RemoveDraftRouteConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}