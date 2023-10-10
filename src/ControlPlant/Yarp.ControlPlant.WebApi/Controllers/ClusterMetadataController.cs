using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp cluster config api
/// </summary>
// [Authorize]
[Route("api/cluster")]
[ApiController]
public class ClusterMetadataController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClusterMetadataController(IMediator mediator)
    {
        this._mediator = mediator;
    }

    /// <summary>
    /// Appends metadata to the cluster.
    /// </summary>
    /// <param name="clusterId"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{clusterId}/Metadata/Append")]
    public async Task<IActionResult> AppendMetadata([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Overrides the metadata of the cluster.
    /// </summary>
    /// <param name="clusterId"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{clusterId}/Metadata/Override")]
    public async Task<IActionResult> OverrideMetadata([FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Overrides a specific metadata of the cluster by key.
    /// </summary>
    /// <param name="clusterId"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{clusterId}/Metadata/Override/{metadataKey}")]
    public async Task<IActionResult> OverrideMetadataByKey([FromRoute] string clusterId, [FromRoute] string metadataKey, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}