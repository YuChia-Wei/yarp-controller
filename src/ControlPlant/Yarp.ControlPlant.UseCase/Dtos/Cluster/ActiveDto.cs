using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.UseCase.Dtos.Cluster;

public class ActiveDto
{
    /// <summary>
    /// Whether active health checks are enabled.
    /// </summary>
    public bool? Enabled { get; set; }

    /// <summary>
    /// Health probe interval.
    /// </summary>
    public TimeSpan? Interval { get; set; }

    /// <summary>
    /// Health probe timeout, after which a destination is considered unhealthy.
    /// </summary>
    public TimeSpan? Timeout { get; set; }

    /// <summary>
    /// Active health check policy.
    /// </summary>
    public ActivePolicy? Policy { get; set; }

    /// <summary>
    /// HTTP health check endpoint path.
    /// </summary>
    public string? Path { get; set; }
}