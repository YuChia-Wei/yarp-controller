using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp cluster config api
/// </summary>
// [Authorize]
[Route("api/cluster")]
[ApiController]
public class ClusterDestinationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClusterDestinationsController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Appends destinations to the cluster.
    /// </summary>
    [HttpPost("{clusterId}/Destinations/Append")]
    public async Task<IActionResult> AppendDestinationsAsync([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Gets the destinations of the cluster.
    /// </summary>
    [HttpGet("{clusterId}/Destinations/Get")]
    public async Task<IActionResult> GetDestinationsAsync([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Overrides a specific destination of the cluster by key.
    /// </summary>
    [HttpPost("{clusterId}/Destinations/Override/{destinationKey}")]
    public async Task<IActionResult> OverrideDestinationByKeyAsync([FromRoute] string clusterId, [FromRoute] string destinationKey, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Overrides the destinations of the cluster.
    /// </summary>
    [HttpPost("{clusterId}/Destinations/Override")]
    public async Task<IActionResult> OverrideDestinationsAsync([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}