using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp routing config api
/// </summary>
// [Authorize]
[Route("api/route")]
[ApiController]
public class MetadataController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <inheritdoc />
    public MetadataController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Adds or modifies metadata for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    [HttpPost("Metadata")]
    public async Task<IActionResult> AddOrUpdateMetadataAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Deletes a specific metadata entry from a route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="metadataKey">The key of the metadata entry to delete.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpDelete("Metadata/{metadataKey}")]
    public async Task<IActionResult> DeleteMetadataAsync([FromRoute] string routeId, [FromRoute] string metadataKey, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves metadata for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("Metadata")]
    public async Task<IActionResult> GetMetadataAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Modifies metadata for a specific route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPut("Metadata")]
    public async Task<IActionResult> UpdateMetadataAsync([FromRoute] string routeId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Modifies a specific metadata entry for a route.
    /// </summary>
    /// <param name="routeId">The identifier of the route.</param>
    /// <param name="metadataKey">The key of the metadata entry to modify.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPut("Metadata/{metadataKey}")]
    public async Task<IActionResult> UpdateSpecificMetadataAsync([FromRoute] string routeId, [FromRoute] string metadataKey, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}