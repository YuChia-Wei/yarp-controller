// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.ControlPlant.Entity.Model;

namespace Yarp.ReverseProxy.ControlPlant.Entity.LoadBalancing;

/// <summary>
/// Provides a method that applies a load balancing policy
/// to select a destination.
/// </summary>
public interface ILoadBalancingPolicy
{
    /// <summary>
    /// A unique identifier for this load balancing policy. This will be referenced from config.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Picks a destination to send traffic to.
    /// </summary>
    DestinationState? PickDestination(HttpContext context, ClusterState cluster, IReadOnlyList<DestinationState> availableDestinations);
}