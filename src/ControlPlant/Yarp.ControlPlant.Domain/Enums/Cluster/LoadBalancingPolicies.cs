namespace Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

/// <summary>
/// Names of built-in load balancing policies.
/// </summary>
public enum LoadBalancingPolicies
{
    /// <summary>
    /// Select the alphabetically first available destination without considering load. This is useful for dual destination
    /// fail-over systems.
    /// </summary>
    FirstAlphabetical = 1,

    /// <summary>
    /// Select a destination randomly.
    /// </summary>
    Random,

    /// <summary>
    /// Select a destination by cycling through them in order.
    /// </summary>
    RoundRobin,

    /// <summary>
    /// Select the destination with the least assigned requests. This requires examining all destinations.
    /// </summary>
    LeastRequests,

    /// <summary>
    /// Select two random destinations and then select the one with the least assigned requests.
    /// This avoids the overhead of LeastRequests and the worst case for Random where it selects a busy destination.
    /// </summary>
    PowerOfTwoChoices
}