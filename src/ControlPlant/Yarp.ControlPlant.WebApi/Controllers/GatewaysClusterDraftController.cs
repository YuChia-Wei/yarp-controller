using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp gateway sync api
/// </summary>
// [Authorize]
[Route("api/gateway")]
[ApiController]
public class GatewaysClusterDraftController : ControllerBase
{
    private readonly IMediator _mediator;

    public GatewaysClusterDraftController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Appends draft cluster configurations to the specified gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{gatewayId}/cluster/draft/Append")]
    public async Task<IActionResult> AppendDraftClusterConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the draft cluster configurations of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("{gatewayId}/cluster/draft/Get")]
    public async Task<IActionResult> GetDraftClusterConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Overrides the draft cluster configurations of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{gatewayId}/cluster/draft/Override")]
    public async Task<IActionResult> OverrideDraftClusterConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Removes the draft cluster configurations of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpDelete("{gatewayId}/cluster/draft/Remove")]
    public async Task<IActionResult> RemoveDraftClusterConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}