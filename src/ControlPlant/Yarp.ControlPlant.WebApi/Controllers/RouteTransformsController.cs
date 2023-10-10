using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp routing config api
/// </summary>
// [Authorize]
[Route("api/route")]
[ApiController]
public class RouteTransformsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RouteTransformsController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Adds a new transform for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("Transforms")]
    public async Task<IActionResult> AddTransformAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Deletes a specific transform from a route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="transformKey">The key of the transform to delete.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpDelete("Transforms/{transformKey}")]
    public async Task<IActionResult> DeleteTransformAsync([FromRoute] string routeId, [FromRoute] string transformKey, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the transforms for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("Transforms")]
    public async Task<IActionResult> GetTransformsAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Modifies the transforms for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPut("Transforms")]
    public async Task<IActionResult> UpdateTransformsAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}