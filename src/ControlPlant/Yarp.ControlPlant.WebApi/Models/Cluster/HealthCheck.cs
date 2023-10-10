using Yarp.ControlPlant.UseCase.Dtos.Cluster;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;

namespace Yarp.ControlPlant.WebApi.Models.Cluster;

public class HealthCheck
{
    public ActiveHealthCheck? Active { get; set; }
    public PassiveHealthCheck? Passive { get; set; }
    public AvailableDestinations? AvailableDestinationsPolicy { get; set; }

    internal HealthCheckDto MapToDto()
    {
        return new HealthCheckDto
        {
            Passive = new PassiveDto
            {
                Enabled = this.Passive?.Enabled,
                Policy = this.Passive?.Policy,
                ReactivationPeriod = this.Passive?.ReactivationPeriod
            },
            Active = new ActiveDto
            {
                Enabled = this.Active?.Enabled,
                Interval = this.Active?.Interval,
                Timeout = this.Active?.Timeout,
                Policy = this.Active?.Policy,
                Path = this.Active?.Path
            },
            AvailableDestinationsPolicy = this.AvailableDestinationsPolicy
        };
    }
}