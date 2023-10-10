using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp gateway sync api
/// </summary>
// [Authorize]
[Route("api/gateway")]
[ApiController]
public class GatewayController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="mediator"></param>
    public GatewayController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Adds a new gateway.
    /// </summary>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost]
    public async Task<IActionResult> AddGateway(CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the information of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("{gatewayId}")]
    public async Task<IActionResult> GetGatewayById([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the list of gateways.
    /// </summary>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet]
    public async Task<IActionResult> GetGateways(CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Updates the information of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPut("{gatewayId}")]
    public async Task<IActionResult> UpdateGateway([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}