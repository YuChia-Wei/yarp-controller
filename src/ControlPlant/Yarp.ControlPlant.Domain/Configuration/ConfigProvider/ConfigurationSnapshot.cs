// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Primitives;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration.ConfigProvider;

internal class ConfigurationSnapshot : IProxyConfig
{
    public List<RouteConfig> Routes { get; internal set; } = new List<RouteConfig>();

    public List<ClusterConfig> Clusters { get; internal set; } = new List<ClusterConfig>();

    IReadOnlyList<RouteConfig> IProxyConfig.Routes => this.Routes;

    IReadOnlyList<ClusterConfig> IProxyConfig.Clusters => this.Clusters;

    // This field is required.
    public IChangeToken ChangeToken { get; internal set; } = default!;
}