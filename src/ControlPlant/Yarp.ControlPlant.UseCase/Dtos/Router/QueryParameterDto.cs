using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Dtos.Router;

/// <summary>
/// Route criteria for a query parameter that must be present on the incoming request.
/// </summary>
public class QueryParameterDto
{
    /// <summary>
    /// Name of the query parameter to look for.
    /// This field is case insensitive and required.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// A collection of acceptable query parameter values used during routing.
    /// </summary>
    public IEnumerable<string>? Values { get; set; }

    /// <summary>
    /// Specifies how query parameter values should be compared (e.g. exact matches Vs. contains).
    /// Defaults to <see cref="QueryParameterMatchMode.Exact" />.
    /// </summary>
    public QueryParameterMatchMode Mode { get; set; }

    /// <summary>
    /// Specifies whether query parameter value comparisons should ignore case.
    /// When <c>true</c>, <see cref="StringComparison.Ordinal" /> is used.
    /// When <c>false</c>, <see cref="StringComparison.OrdinalIgnoreCase" /> is used.
    /// Defaults to <c>false</c>.
    /// </summary>
    public bool IsCaseSensitive { get; set; }
}