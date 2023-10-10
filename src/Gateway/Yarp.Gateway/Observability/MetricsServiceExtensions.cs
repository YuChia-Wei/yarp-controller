using Yarp.Gateway.Observability.Metrics;
using Yarp.Telemetry.Consumption;

namespace Yarp.Gateway.Observability;

public static class YarpMetricsExtensions
{
    public static IServiceCollection AddYarpDnsMetrics(this IServiceCollection services)
    {
        services.AddTelemetryListeners();
        services.AddSingleton<IMetricsConsumer<NameResolutionMetrics>, YarpDnsMetrics>();
        return services;
    }

    public static IServiceCollection AddYarpForwarderMetrics(this IServiceCollection services)
    {
        services.AddTelemetryListeners();
        services.AddSingleton<IMetricsConsumer<ForwarderMetrics>, YarpForwarderMetrics>();
        return services;
    }

    public static IServiceCollection AddYarpKestrelMetrics(this IServiceCollection services)
    {
        services.AddTelemetryListeners();
        services.AddSingleton<IMetricsConsumer<KestrelMetrics>, YarpKestrelMetrics>();
        return services;
    }

    public static IServiceCollection AddYarpMetrics(this IServiceCollection services)
    {
        services.AddYarpForwarderMetrics();
        services.AddYarpDnsMetrics();
        services.AddYarpKestrelMetrics();
        services.AddYarpOutboundHttpMetrics();
        services.AddYarpSocketsMetrics();
        return services;
    }

    public static IServiceCollection AddYarpOutboundHttpMetrics(this IServiceCollection services)
    {
        services.AddTelemetryListeners();
        services.AddSingleton<IMetricsConsumer<HttpMetrics>, YarpOutboundHttpMetrics>();
        return services;
    }

    public static IServiceCollection AddYarpSocketsMetrics(this IServiceCollection services)
    {
        services.AddTelemetryListeners();
        services.AddSingleton<IMetricsConsumer<SocketsMetrics>, YarpSocketMetrics>();
        return services;
    }
}