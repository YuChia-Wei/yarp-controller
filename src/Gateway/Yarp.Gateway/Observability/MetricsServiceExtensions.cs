using Yarp.Gateway.Observability.Metrics;
using Yarp.Telemetry.Consumption;

namespace Yarp.Gateway.Observability;

public static class YarpMetricsExtensions
{
    public static IServiceCollection AddYarpMetrics(this IServiceCollection services)
    {
        services.AddTelemetryListeners();
        services.AddSingleton<IMetricsConsumer<ForwarderMetrics>, YarpForwarderMetrics>();
        services.AddSingleton<IMetricsConsumer<NameResolutionMetrics>, YarpDnsMetrics>();
        services.AddSingleton<IMetricsConsumer<KestrelMetrics>, YarpKestrelMetrics>();
        services.AddSingleton<IMetricsConsumer<HttpMetrics>, YarpOutboundHttpMetrics>();
        services.AddSingleton<IMetricsConsumer<SocketsMetrics>, YarpSocketMetrics>();
        return services;
    }
}