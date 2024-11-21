using Yarp.ReverseProxy.Configuration;

namespace Yarp.Gateway.YarpComponents;

public static class IProxyConfigExtenstion
{
    public static GatewayConfig ToGatewayConfig(this IProxyConfig proxyConfig)
    {
        return new GatewayConfig(proxyConfig);
    }
}