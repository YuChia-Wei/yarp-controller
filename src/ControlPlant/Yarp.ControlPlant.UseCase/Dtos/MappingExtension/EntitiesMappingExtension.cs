using System.Security.Authentication;
using Yarp.ControlPlant.UseCase.Dtos.Cluster;
using Yarp.ControlPlant.UseCase.Dtos.Router;
using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Cluster;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Router;
using Yarp.ReverseProxy.ControlPlant.Entity.Forwarder;

namespace Yarp.ControlPlant.UseCase.Dtos.MappingExtension;

public static class EntitiesMappingExtension
{
    public static HealthCheckDto MapToDto(this HealthCheckConfig healthCheckConfig)
    {
        return new HealthCheckDto
        {
            Passive = new PassiveDto()
            {
                Enabled = healthCheckConfig.Passive?.Enabled,
                Policy = healthCheckConfig.Passive?.Policy is null
                             ? Enum.Parse<PassivePolicy>(healthCheckConfig.Passive?.Policy ?? string.Empty)
                             : null,
                ReactivationPeriod = healthCheckConfig.Passive?.ReactivationPeriod
            },
            Active = new ActiveDto()
            {
                Enabled = healthCheckConfig.Active?.Enabled,
                Interval = healthCheckConfig.Active?.Interval,
                Timeout = healthCheckConfig.Active?.Timeout,
                Policy = healthCheckConfig.Active?.Policy is null
                             ? Enum.Parse<ActivePolicy>(healthCheckConfig.Passive?.Policy ?? string.Empty)
                             : null,
                Path = healthCheckConfig.Active?.Path
            },
            AvailableDestinationsPolicy = healthCheckConfig.AvailableDestinationsPolicy is null
                                              ? Enum.Parse<AvailableDestinations>(healthCheckConfig.AvailableDestinationsPolicy ?? string.Empty)
                                              : null
        };
    }

    public static HttpRequestDto MapToDto(this ForwarderRequestConfig forwarderRequestConfig)
    {
        return new HttpRequestDto
        {
            ActivityTimeout = forwarderRequestConfig.ActivityTimeout,
            Version = forwarderRequestConfig.Version,
            VersionPolicy = forwarderRequestConfig.VersionPolicy,
            AllowResponseBuffering = forwarderRequestConfig.AllowResponseBuffering
        };
    }

    public static HttpClientDto MapToDto(this HttpClientConfig httpClientConfig)
    {
        return new HttpClientDto
        {
            SslProtocols = httpClientConfig.SslProtocols != null
                               ? Enum.GetValues(typeof(SslProtocols))
                                     .Cast<SslProtocols>()
                                     .Where(item => (httpClientConfig.SslProtocols & item) == item)
                               : null,
            DangerousAcceptAnyServerCertificate = httpClientConfig.DangerousAcceptAnyServerCertificate,
            MaxConnectionsPerServer = httpClientConfig.MaxConnectionsPerServer,
            WebProxy = new WebProxyDto
            {
                Address = httpClientConfig.WebProxy?.Address,
                BypassOnLocal = httpClientConfig.WebProxy?.BypassOnLocal,
                UseDefaultCredentials = httpClientConfig.WebProxy?.UseDefaultCredentials
            },
            EnableMultipleHttp2Connections = httpClientConfig.EnableMultipleHttp2Connections,
            RequestHeaderEncoding = httpClientConfig.RequestHeaderEncoding
        };
    }

    public static DestinationDto MapToDto(this DestinationConfig destinationConfig)
    {
        return new DestinationDto
        {
            Address = destinationConfig.Address,
            Health = destinationConfig?.Health,
            Metadata = destinationConfig?.Metadata
        };
    }

    public static MatchDto MapToDto(this RouteMatch routeMatch)
    {
        return new MatchDto
        {
            Methods = routeMatch.Methods?.Select(o => Enum.Parse<HttpRequestMethod>(o.ToString())),
            Hosts = routeMatch.Hosts?.ToList().AsReadOnly(),
            Path = routeMatch.Path,
            QueryParameters = routeMatch.QueryParameters?.Select(o => new QueryParameterDto
            {
                Name = o.Name!,
                Values = o.Values?.ToList().AsReadOnly(),
                Mode = o.Mode,
                IsCaseSensitive = o.IsCaseSensitive
            }).ToList().AsReadOnly(),
            Headers = routeMatch.Headers?.Select(o => new HeaderDto
            {
                Name = o.Name!,
                Values = o.Values?.ToList().AsReadOnly(),
                Mode = o.Mode,
                IsCaseSensitive = o.IsCaseSensitive
            }).ToList().AsReadOnly()
        };
    }

    public static SessionAffinityDto MapToDto(this SessionAffinityConfig sessionAffinityConfig)
    {
        return new SessionAffinityDto
        {
            Enabled = sessionAffinityConfig.Enabled ?? null,
            Policy = sessionAffinityConfig.Policy is null ? Enum.Parse<SessionAffinityPolicy>(sessionAffinityConfig.Policy ?? string.Empty) : null,
            FailurePolicy =
                sessionAffinityConfig.FailurePolicy is null
                    ? Enum.Parse<SessionAffinityFailurePolicy>(sessionAffinityConfig.FailurePolicy ?? string.Empty)
                    : null,
            AffinityKeyName = sessionAffinityConfig.AffinityKeyName!,
            Cookie = sessionAffinityConfig.Cookie != null
                         ? new CookieDto
                         {
                             Path = sessionAffinityConfig.Cookie.Path,
                             Domain = sessionAffinityConfig.Cookie.Domain,
                             HttpOnly = sessionAffinityConfig.Cookie.HttpOnly,
                             SecurePolicy = sessionAffinityConfig.Cookie.SecurePolicy,
                             SameSite = sessionAffinityConfig.Cookie.SameSite,
                             Expiration = sessionAffinityConfig.Cookie.Expiration,
                             MaxAge = sessionAffinityConfig.Cookie.MaxAge,
                             IsEssential = sessionAffinityConfig.Cookie.IsEssential
                         }
                         : null
        };
    }
}