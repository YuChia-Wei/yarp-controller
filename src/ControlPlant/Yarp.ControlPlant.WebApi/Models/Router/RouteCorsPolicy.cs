namespace Yarp.ControlPlant.WebApi.Models.Router;

/// <summary>
/// Represents the CORS policies for routes.
/// </summary>
public enum RouteCorsPolicy
{
    /// <summary>
    /// The default CORS policy.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Represents a policy where CORS is disabled.
    /// </summary>
    Disable = 1
}