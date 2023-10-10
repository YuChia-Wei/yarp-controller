using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp routing config api
/// </summary>
// [Authorize]
[Route("api/route")]
[ApiController]
public class RouteMatchController : ControllerBase
{
    private readonly IMediator _mediator;

    public RouteMatchController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Adds or modifies matching conditions for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("Match")]
    public async Task<IActionResult> AddOrUpdateMatchAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the matching conditions for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("Match")]
    public async Task<IActionResult> GetMatchAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Modifies matching conditions for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPut("Match")]
    public async Task<IActionResult> UpdateMatchAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}