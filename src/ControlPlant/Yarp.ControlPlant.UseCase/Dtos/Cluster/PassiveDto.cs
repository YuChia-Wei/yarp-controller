using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.UseCase.Dtos.Cluster;

public class PassiveDto
{
    /// <summary>
    /// Whether passive health checks are enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Passive health check policy.
    /// </summary>
    public PassivePolicy? Policy { get; set; }

    /// <summary>
    /// Destination reactivation period after which an unhealthy destination is considered healthy again.
    /// </summary>
    public TimeSpan? ReactivationPeriod { get; set; }
}