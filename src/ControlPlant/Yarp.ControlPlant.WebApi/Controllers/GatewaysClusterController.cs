using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// yarp gateway sync api
/// </summary>
// [Authorize]
[Route("api/gateway")]
[ApiController]
public class GatewaysClusterController : ControllerBase
{
    /// <summary>
    /// 將指定叢集設定加入至指定 gateway
    /// </summary>
    /// <param name="gatewayId"></param>
    /// <param name="clusterId"></param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns></returns>
    [HttpPost("{gatewayId}/cluster/{clusterId}")]
    public async Task<ActionResult> AddClusterSettingToGatewayAsync([FromRoute] string gatewayId, [FromRoute] string clusterId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Applies cluster configurations to the specified gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpPost("{gatewayId}/cluster/Apply")]
    public async Task<IActionResult> ApplyClusterConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }

    /// <summary>
    /// Retrieves the online cluster configurations of a specific gateway.
    /// </summary>
    /// <param name="gatewayId">The identifier of the gateway.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    [HttpGet("{gatewayId}/cluster/Online")]
    public async Task<IActionResult> GetOnlineClusterConfig([FromRoute] string gatewayId, CancellationToken cancellationToken)
    {
        return this.Ok();
    }
}