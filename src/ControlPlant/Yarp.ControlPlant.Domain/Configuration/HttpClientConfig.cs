// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Authentication;

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Options used for communicating with the destination servers.
/// </summary>
public class HttpClientConfig
{
    /// <summary>
    /// An empty options instance.
    /// </summary>
    public static readonly HttpClientConfig Empty = new();

    /// <summary>
    /// What TLS protocols to use.
    /// </summary>
    public SslProtocols? SslProtocols { get; set; }

    /// <summary>
    /// Indicates if destination server https certificate errors should be ignored.
    /// This should only be done when using self-signed certificates.
    /// </summary>
    public bool? DangerousAcceptAnyServerCertificate { get; set; }

    /// <summary>
    /// Limits the number of connections used when communicating with the destination server.
    /// </summary>
    public int? MaxConnectionsPerServer { get; set; }

    /// <summary>
    /// Optional web proxy used when communicating with the destination server.
    /// </summary>
    public WebProxyConfig? WebProxy { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether additional HTTP/2 connections can
    /// be established to the same server when the maximum number of concurrent streams
    /// is reached on all existing connections.
    /// </summary>
    public bool? EnableMultipleHttp2Connections { get; set; }

    /// <summary>
    /// Enables non-ASCII header encoding for outgoing requests.
    /// </summary>
    public string? RequestHeaderEncoding { get; set; }

    public bool Equals(HttpClientConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.SslProtocols == other.SslProtocols
               && this.DangerousAcceptAnyServerCertificate == other.DangerousAcceptAnyServerCertificate
               && this.MaxConnectionsPerServer == other.MaxConnectionsPerServer
               && this.EnableMultipleHttp2Connections == other.EnableMultipleHttp2Connections
               // Comparing by reference is fine here since Encoding.GetEncoding returns the same instance for each encoding.
               && this.RequestHeaderEncoding == other.RequestHeaderEncoding
               && this.WebProxy == other.WebProxy;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.SslProtocols, this.DangerousAcceptAnyServerCertificate, this.MaxConnectionsPerServer, this.EnableMultipleHttp2Connections,
                                this.RequestHeaderEncoding, this.WebProxy);
    }
}