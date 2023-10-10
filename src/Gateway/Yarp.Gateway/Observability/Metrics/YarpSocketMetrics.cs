using System.Diagnostics.Metrics;
using Yarp.Telemetry.Consumption;

namespace Yarp.Gateway.Observability.Metrics;

public sealed class YarpSocketMetrics : IMetricsConsumer<SocketsMetrics>
{
    private static readonly Counter<long> OutgoingConnectionsEstablished = GatewayMeter.Meter.CreateCounter<long>(
        "yarp_sockets_outgoing_connections_established",
        description: "Number of outgoing (Connect) Socket connections established"
    );

    private static readonly Counter<long> IncomingConnectionsEstablished = GatewayMeter.Meter.CreateCounter<long>(
        "yarp_sockets_incoming_connections_established",
        description: "Number of incoming (Accept) Socket connections established"
    );

    private static readonly Counter<long> BytesReceived = GatewayMeter.Meter.CreateCounter<long>(
        "yarp_sockets_bytes_received",
        "bytes",
        "Number of bytes received"
    );

    private static readonly Counter<long> BytesSent = GatewayMeter.Meter.CreateCounter<long>(
        "yarp_sockets_bytes_sent",
        "bytes",
        "Number of bytes sent"
    );

    private static readonly Counter<long> DatagramsReceived = GatewayMeter.Meter.CreateCounter<long>(
        "yarp_sockets_datagrams_received",
        description: "Number of datagrams received"
    );

    private static readonly Counter<long> DatagramsSent = GatewayMeter.Meter.CreateCounter<long>(
        "yarp_sockets_datagrams_sent",
        description: "Number of datagrams Sent"
    );

    public void OnMetrics(SocketsMetrics previous, SocketsMetrics current)
    {
        OutgoingConnectionsEstablished.Add(current.OutgoingConnectionsEstablished);
        IncomingConnectionsEstablished.Add(current.IncomingConnectionsEstablished);
        BytesReceived.Add(current.BytesReceived);
        BytesSent.Add(current.BytesSent);
        DatagramsReceived.Add(current.DatagramsReceived);
        DatagramsSent.Add(current.DatagramsSent);
    }
}