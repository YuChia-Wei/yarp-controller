namespace Yarp.ControlPlant.WebApi.Models.Router;

public class Router
{
    /// <summary>
    /// 路由說明
    /// </summary>
    public string? RouterDescription { get; set; }

    /// <summary>
    /// Gets or sets the cluster that requests matching this route
    /// should be proxied to.
    /// </summary>
    public string? ClusterId { get; set; }

    /// <summary>
    /// Optionally, an order value for this route. Routes with lower numbers take precedence over higher numbers.
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// An optional override for how large request bodies can be in bytes. If set, this overrides the server's default (30MB)
    /// per request.
    /// Set to '-1' to disable the limit for this route.
    /// </summary>
    public long? MaxRequestBodySize { get; set; }

    /// <summary>
    /// The name of the AuthorizationPolicy to apply to this route.
    /// If not set then only the FallbackPolicy will apply.
    /// Set to "Default" to enable authorization with the applications default policy.
    /// Set to "Anonymous" to disable all authorization checks for this route.
    /// </summary>
    public RouteAuthorizationPolicy AuthorizationPolicy { get; set; }

    /// <summary>
    /// The name of the RateLimiterPolicy to apply to this route.
    /// If not set then only the GlobalLimiter will apply.
    /// Set to "Disable" to disable rate limiting for this route.
    /// Set to "Default" or leave empty to use the global rate limits, if any.
    /// </summary>
    public string? RateLimiterPolicy { get; set; }

    /// <summary>
    /// The name of the CorsPolicy to apply to this route.
    /// If not set then the route won't be automatically matched for cors preflight requests.
    /// Set to "Default" to enable cors with the default policy.
    /// Set to "Disable" to refuses cors requests for this route.
    /// </summary>
    public string? CorsPolicy { get; set; }

    /// <summary>
    /// Parameters used to match requests.
    /// This field is required.
    /// </summary>
    public MatchRule MatchRule { get; set; } = default!;

    /// <summary>
    /// Parameters used to transform the request and response. See <see cref="ImmutableArray{T}.Builder.ITransformBuilder" />.
    /// </summary>
    public IReadOnlyList<IReadOnlyDictionary<string, string>>? Transforms { get; set; }

    /// <summary>
    /// Arbitrary key-value pairs that further describe this route.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; set; }
}