using System.Security.Authentication;
using Yarp.ControlPlant.UseCase.Dtos.Cluster;

namespace Yarp.ControlPlant.WebApi.Models.Cluster;

public class HttpClientSetting
{
    /// <summary>
    /// What TLS protocols to use.
    /// </summary>
    public IEnumerable<SslProtocols>? SslProtocols { get; set; }

    /// <summary>
    /// Limits the number of connections used when communicating with the destination server.
    /// </summary>
    public int? MaxConnectionsPerServer { get; set; }

    /// <summary>
    /// Indicates if destination server https certificate errors should be ignored.
    /// This should only be done when using self-signed certificates.
    /// </summary>
    public bool? DangerousAcceptAnyServerCertificate { get; set; }

    /// <summary>
    /// Enables non-ASCII header encoding for outgoing requests.
    /// </summary>
    public string? RequestHeaderEncoding { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether additional HTTP/2 connections can
    /// be established to the same server when the maximum number of concurrent streams
    /// is reached on all existing connections.
    /// </summary>
    public bool? EnableMultipleHttp2Connections { get; set; }

    /// <summary>
    /// Optional web proxy used when communicating with the destination server.
    /// </summary>
    public ProxyServer? WebProxy { get; set; }

    internal HttpClientDto MapToDto()
    {
        return new HttpClientDto
        {
            SslProtocols = this.SslProtocols,
            DangerousAcceptAnyServerCertificate = this.DangerousAcceptAnyServerCertificate,
            MaxConnectionsPerServer = this.MaxConnectionsPerServer,
            WebProxy = new WebProxyDto
            {
                Address = this.WebProxy?.Address,
                BypassOnLocal = this.WebProxy?.BypassOnLocal,
                UseDefaultCredentials = this.WebProxy?.UseDefaultCredentials
            },
            EnableMultipleHttp2Connections = this.EnableMultipleHttp2Connections,
            RequestHeaderEncoding = this.RequestHeaderEncoding
        };
    }
}