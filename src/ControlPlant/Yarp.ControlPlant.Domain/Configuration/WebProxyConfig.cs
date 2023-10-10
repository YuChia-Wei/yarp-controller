// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

/// <summary>
/// Config used to construct <seealso cref="System.Net.WebProxy" /> instance.
/// </summary>
public class WebProxyConfig : IEquatable<WebProxyConfig>
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

    public bool Equals(WebProxyConfig? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Address == other.Address
               && this.BypassOnLocal == other.BypassOnLocal
               && this.UseDefaultCredentials == other.UseDefaultCredentials;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Address, this.BypassOnLocal, this.UseDefaultCredentials
        );
    }
}