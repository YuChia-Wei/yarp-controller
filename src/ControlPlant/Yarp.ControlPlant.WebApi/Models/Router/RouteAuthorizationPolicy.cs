namespace Yarp.ControlPlant.WebApi.Models.Router;

/// <summary>
/// Represents the authorization policies for routes.
/// </summary>
public enum RouteAuthorizationPolicy
{
    /// <summary>
    /// The default authorization policy.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Represents a policy where no authorization is required.
    /// </summary>
    Anonymous = 1
}