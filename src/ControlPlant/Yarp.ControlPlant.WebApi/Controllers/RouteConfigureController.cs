using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp routing config api
/// </summary>
// [Authorize]
[Route("api/route")]
[ApiController]
public class RouteConfigureController : ControllerBase
{
    private readonly IMediator _mediator;

    public RouteConfigureController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Configures the authorization for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("authorization")]
    public async Task<IActionResult> ConfigureAuthorizationAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Configures the cluster association for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("cluster")]
    public async Task<IActionResult> ConfigureClusterAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Configures the CORS settings for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("cors")]
    public async Task<IActionResult> ConfigureCorsAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Configures the rate limiter for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("rateLimiter")]
    public async Task<IActionResult> ConfigureRateLimiterAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Configures the request size limits for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("requestSize")]
    public async Task<IActionResult> ConfigureRequestSizeAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Sets the description for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("description")]
    public async Task<IActionResult> SetDescriptionAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Sets the order for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("order")]
    public async Task<IActionResult> SetOrderAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}