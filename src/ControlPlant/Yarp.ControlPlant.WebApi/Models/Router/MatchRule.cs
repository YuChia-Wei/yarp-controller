using Yarp.ControlPlant.UseCase.Dtos.Router;
using Yarp.ReverseProxy.ControlPlant.Entity.Enums.Router;

namespace Yarp.ControlPlant.WebApi.Models.Router;

/// <summary>
/// Describes the matching criteria for a route.
/// </summary>
public class MatchRule
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
    public IEnumerable<Header>? Headers { get; set; }

    /// <summary>
    /// Only match requests that contain all of these query parameters.
    /// </summary>
    public IEnumerable<HttpQueryParameter>? QueryParameters { get; set; }

    internal MatchDto MapToDto()
    {
        return new MatchDto
        {
            Methods = this.Methods,
            Hosts = this.Hosts?.ToList().AsReadOnly(),
            Path = this.Path,
            QueryParameters = this.QueryParameters?.Select(o => new QueryParameterDto
            {
                Name = o.Name!,
                Values = o.Values?.ToList().AsReadOnly(),
                Mode = o.Mode,
                IsCaseSensitive = o.IsCaseSensitive
            }).ToList().AsReadOnly(),
            Headers = this.Headers?.Select(o => new HeaderDto
            {
                Name = o.Name!,
                Values = o.Values?.ToList().AsReadOnly(),
                Mode = o.Mode,
                IsCaseSensitive = o.IsCaseSensitive
            }).ToList().AsReadOnly()
        };
    }
}