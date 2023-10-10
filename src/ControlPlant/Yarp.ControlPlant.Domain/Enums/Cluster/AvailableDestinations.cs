namespace Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

public enum AvailableDestinations
{
    /// <summary>
    /// Marks destination as available for proxying requests to if its health state
    /// is either 'Healthy' or 'Unknown'. If no destinations are available then
    /// requests will get a 503 error.
    /// </summary>
    /// <remarks>It applies only if active or passive health checks are enabled.</remarks>
    HealthyAndUnknown = 0,

    /// <summary>
    /// Calls <see cref="HealthyAndUnknown" /> policy at first to determine
    /// destinations' availability. If no available destinations are returned
    /// from this call, it marks all cluster's destination as available.
    /// </summary>
    HealthyOrPanic
}