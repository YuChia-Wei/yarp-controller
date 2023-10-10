using System.Diagnostics.Metrics;

namespace Yarp.Gateway.Observability.Metrics;

/// <summary>
/// https://learn.microsoft.com/zh-tw/dotnet/core/diagnostics/compare-metric-apis#systemdiagnosticsmetrics
/// </summary>
public static class GatewayMeter
{
    public static readonly Meter Meter = new("yarp.gateway.observability.library");
}