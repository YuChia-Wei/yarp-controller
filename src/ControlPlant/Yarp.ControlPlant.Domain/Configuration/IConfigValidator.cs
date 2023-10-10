// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Provides methods to validate routes and clusters.
/// </summary>
public interface IConfigValidator
{
    /// <summary>
    /// Validates a cluster and returns all errors.
    /// </summary>
    ValueTask<IList<Exception>> ValidateClusterAsync(ClusterConfig cluster);

    /// <summary>
    /// Validates a route and returns all errors
    /// </summary>
    ValueTask<IList<Exception>> ValidateRouteAsync(RouteConfig route);
}