using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.UseCase.Dtos.Cluster;

public class HealthCheckDto
{
    public ActiveDto? Active { get; set; }
    public PassiveDto? Passive { get; set; }
    public AvailableDestinations? AvailableDestinationsPolicy { get; set; }

    public HealthCheckConfig MapToEntity()
    {
        return new HealthCheckConfig
        {
            Passive = new PassiveHealthCheckConfig
            {
                Enabled = this.Passive?.Enabled,
                Policy = this.Passive?.Policy.ToString(),
                ReactivationPeriod = this.Passive?.ReactivationPeriod
            },
            Active = new ActiveHealthCheckConfig
            {
                Enabled = this.Active?.Enabled,
                Interval = this.Active?.Interval,
                Timeout = this.Active?.Timeout,
                Policy = this.Active?.Policy.ToString(),
                Path = this.Active?.Path
            },
            AvailableDestinationsPolicy = this.AvailableDestinationsPolicy.ToString()
        };
    }
}