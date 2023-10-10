namespace Yarp.ControlPlant.WebApi.Models.Cluster;

/// <summary>
/// Config used to construct <seealso cref="System.Net.WebProxy" /> instance.
/// </summary>
public class ProxyServer
{
    /// <summary>
    /// The URI of the proxy server.
    /// </summary>
    public Uri? Address { get; set; }

    /// <summary>
    /// true to bypass the proxy for local addresses; otherwise, false.
    /// If null, default value will be used: false
    /// </summary>
    public bool? BypassOnLocal { get; set; }

    /// <summary>
    /// Controls whether the <seealso cref="System.Net.CredentialCache.DefaultCredentials" /> are sent with requests.
    /// If null, default value will be used: false
    /// </summary>
    public bool? UseDefaultCredentials { get; set; }
}