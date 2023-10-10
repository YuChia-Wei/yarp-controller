using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Router;

namespace Yarp.ControlPlant.UseCase.Dtos.Router;

/// <summary>
/// Describes the matching criteria for a route.
/// </summary>
public class MatchDto
{
    /// <summary>
    /// Only match requests with the given Path pattern.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Only match requests with the given Host header.
    /// Supports wildcards and ports. For unicode host names, do not use punycode.
    /// </summary>
    public IEnumerable<string>? Hosts { get; set; }

    /// <summary>
    /// Only match requests that use these optional HTTP methods. E.g. GET, POST.
    /// </summary>
    public IEnumerable<HttpRequestMethod>? Methods { get; set; }

    /// <summary>
    /// Only match requests that contain all of these headers.
    /// </summary>
    public IEnumerable<HeaderDto>? Headers { get; set; }

    /// <summary>
    /// Only match requests that contain all of these query parameters.
    /// </summary>
    public IEnumerable<QueryParameterDto>? QueryParameters { get; set; }

    public RouteMatch MapToEntity()
    {
        return new RouteMatch
        {
            Methods = this.Methods?.Select(o => o.ToString()).ToList().AsReadOnly(),
            Hosts = this.Hosts?.ToList().AsReadOnly(),
            Path = this.Path,
            QueryParameters = this.QueryParameters?.Select(o => new RouteQueryParameter
            {
                Name = o.Name!,
                Values = o.Values?.ToList().AsReadOnly(),
                Mode = o.Mode,
                IsCaseSensitive = o.IsCaseSensitive
            }).ToList().AsReadOnly(),
            Headers = this.Headers?.Select(o => new RouteHeader
            {
                Name = o.Name!,
                Values = o.Values?.ToList().AsReadOnly(),
                Mode = o.Mode,
                IsCaseSensitive = o.IsCaseSensitive
            }).ToList().AsReadOnly()
        };
    }
}