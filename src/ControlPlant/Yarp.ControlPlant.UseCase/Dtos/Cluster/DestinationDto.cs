using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Dtos.Cluster;

/// <summary>
/// Describes a destination of a cluster.
/// </summary>
public class DestinationDto
{
    /// <summary>
    /// Address of this destination. E.g. <c>https://127.0.0.1:123/abcd1234/</c>.
    /// This field is required.
    /// </summary>
    public string Address { get; set; } = default!;

    /// <summary>
    /// Endpoint accepting active health check probes. E.g. <c>http://127.0.0.1:1234/</c>.
    /// </summary>
    public string? Health { get; set; }

    /// <summary>
    /// Arbitrary key-value pairs that further describe this destination.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }

    public DestinationConfig MapToEntity()
    {
        return new DestinationConfig
        {
            Address = this.Address,
            Health = this.Health,
            Metadata = this.Metadata
        };
    }
}