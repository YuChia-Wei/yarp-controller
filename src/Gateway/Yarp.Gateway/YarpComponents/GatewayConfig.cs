using Yarp.ReverseProxy.Configuration;

namespace Yarp.Gateway.YarpComponents;

/// <summary>
/// Gateway 設定
/// </summary>
public record GatewayConfig
{
    public GatewayConfig(IProxyConfig getConfig)
    {
        this.Routes = getConfig.Routes;
        this.Clusters = getConfig.Clusters;
    }

    public string ProxyHost { get; } = Environment.MachineName;
    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
}