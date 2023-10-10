using Yarp.ControlPlant.UseCase.Dtos.Cluster;
using Yarp.ControlPlant.UseCase.Dtos.Router;
using Yarp.ControlPlant.WebApi.Models.Cluster;
using Yarp.ControlPlant.WebApi.Models.Router;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Router;

namespace Yarp.ControlPlant.WebApi.Models.MappingExtension;

public static class EntitiesMappingExtension
{
    public static HealthCheck MapToViewModel(this HealthCheckDto healthCheckConfig)
    {
        return new HealthCheck
        {
            Passive = new PassiveHealthCheck()
            {
                Enabled = healthCheckConfig.Passive?.Enabled,
                Policy = healthCheckConfig.Passive?.Policy,
                ReactivationPeriod = healthCheckConfig.Passive?.ReactivationPeriod
            },
            Active = new ActiveHealthCheck()
            {
                Enabled = healthCheckConfig.Active?.Enabled,
                Interval = healthCheckConfig.Active?.Interval,
                Timeout = healthCheckConfig.Active?.Timeout,
                Policy = healthCheckConfig.Active?.Policy,
                Path = healthCheckConfig.Active?.Path
            },
            AvailableDestinationsPolicy = healthCheckConfig.AvailableDestinationsPolicy
        };
    }

    public static HttpRequestSetting MapToViewModel(this HttpRequestDto forwarderRequestConfig)
    {
        return new HttpRequestSetting
        {
            ActivityTimeout = forwarderRequestConfig.ActivityTimeout,
            Version = forwarderRequestConfig.Version,
            VersionPolicy = forwarderRequestConfig.VersionPolicy,
            AllowResponseBuffering = forwarderRequestConfig.AllowResponseBuffering
        };
    }

    public static HttpClientSetting MapToViewModel(this HttpClientDto httpClientConfig)
    {
        return new HttpClientSetting
        {
            SslProtocols = httpClientConfig.SslProtocols,
            DangerousAcceptAnyServerCertificate = httpClientConfig.DangerousAcceptAnyServerCertificate,
            MaxConnectionsPerServer = httpClientConfig.MaxConnectionsPerServer,
            WebProxy = new ProxyServer
            {
                Address = httpClientConfig.WebProxy?.Address,
                BypassOnLocal = httpClientConfig.WebProxy?.BypassOnLocal,
                UseDefaultCredentials = httpClientConfig.WebProxy?.UseDefaultCredentials
            },
            EnableMultipleHttp2Connections = httpClientConfig.EnableMultipleHttp2Connections,
            RequestHeaderEncoding = httpClientConfig.RequestHeaderEncoding
        };
    }

    public static Destination MapToViewModel(this DestinationDto destinationConfig)
    {
        return new Destination
        {
            Address = destinationConfig.Address,
            Health = destinationConfig?.Health,
            Metadata = destinationConfig?.Metadata
        };
    }

    public static MatchRule MapToViewModel(this MatchDto routeMatch)
    {
        return new MatchRule
        {
            Methods = routeMatch.Methods?.Select(o => Enum.Parse<HttpRequestMethod>(o.ToString())),
            Hosts = routeMatch.Hosts?.ToList().AsReadOnly(),
            Path = routeMatch.Path,
            QueryParameters = routeMatch.QueryParameters?.Select(o => new HttpQueryParameter
            {
                Name = o.Name!,
                Values = o.Values?.ToList().AsReadOnly(),
                Mode = o.Mode,
                IsCaseSensitive = o.IsCaseSensitive
            }).ToList().AsReadOnly(),
            Headers = routeMatch.Headers?.Select(o => new Header
            {
                Name = o.Name!,
                Values = o.Values?.ToList().AsReadOnly(),
                Mode = o.Mode,
                IsCaseSensitive = o.IsCaseSensitive
            }).ToList().AsReadOnly()
        };
    }

    public static SessionAffinity MapToViewModel(this SessionAffinityDto sessionAffinityConfig)
    {
        return new SessionAffinity
        {
            Enabled = sessionAffinityConfig.Enabled,
            Policy = sessionAffinityConfig.Policy,
            FailurePolicy =
                sessionAffinityConfig.FailurePolicy,
            AffinityKeyName = sessionAffinityConfig.AffinityKeyName!,
            Cookie = sessionAffinityConfig.Cookie != null
                         ? new CookieConfig
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