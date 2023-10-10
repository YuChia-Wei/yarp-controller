namespace Yarp.ControlPlant.WebApi.Models.Router;

/// <summary>
/// Represents the rate limiter policies for routes.
/// </summary>
public enum RouteRateLimiterPolicy
{
    /// <summary>
    /// The default rate limiter policy.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Represents a policy where rate limiting is disabled.
    /// </summary>
    Disable = 1
}