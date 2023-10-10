// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Model;

/// <summary>
/// Immutable representation of the portions of a route
/// that only change in reaction to configuration changes.
/// </summary>
/// <remarks>
/// All members must remain immutable to avoid thread safety issues.
/// Instead, instances of <see cref="RouteModel" /> are replaced
/// in their entirety when values need to change.
/// </remarks>
public sealed class RouteModel
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public RouteModel(
        RouteConfig config,
        ClusterState? cluster)
    {
        this.Config = config ?? throw new ArgumentNullException(nameof(config));
        this.Cluster = cluster;
    }

    // May not be populated if the cluster config is missing. https://github.com/microsoft/reverse-proxy/issues/797
    /// <summary>
    /// The <see cref="ClusterState" /> instance associated with this route.
    /// </summary>
    public ClusterState? Cluster { get; }

    /// <summary>
    /// The configuration data used to build this route.
    /// </summary>
    public RouteConfig Config { get; }

    internal bool HasConfigChanged(RouteConfig newConfig, ClusterState? cluster, int? routeRevision)
    {
        return this.Cluster != cluster || routeRevision != cluster?.Revision || !this.Config.Equals(newConfig);
    }
}