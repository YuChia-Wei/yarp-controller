using System.Net.Http.Headers;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Yarp.Gateway.YarpComponents.TransformProviders;

internal class AuthenticationTokenTransformProvider : ITransformProvider
{
    /// <summary>Validates any route data needed for transforms.</summary>
    /// <param name="context">The context to add any generated errors to.</param>
    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    /// <summary>Validates any cluster data needed for transforms.</summary>
    /// <param name="context">The context to add any generated errors to.</param>
    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }

    /// <summary>
    /// Inspect the given route and conditionally add transforms.
    /// This is called for every route, each time that route is built.
    /// </summary>
    /// <param name="context">The context to add any generated transforms to.</param>
    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(async transformContext =>
        {
            await SetBearerTokenAsync(transformContext);
        });
    }

    private static async Task SetBearerTokenAsync(RequestTransformContext transformContext)
    {
        if (!transformContext.HttpContext.User.Identities.Any(identity => identity.IsAuthenticated))
        {
            return;
        }

        var resolution = await DownstreamAccessTokenResolver
                               .ResolveAsync(transformContext.HttpContext)
                               .ConfigureAwait(false);
        ApplyAuthorization(transformContext.ProxyRequest, resolution);
    }

    /// <summary>
    /// 將 downstream token 解析結果套用至反向代理要求的 Authorization header。
    /// </summary>
    /// <param name="proxyRequest">即將傳送至下游服務的 HTTP 要求。</param>
    /// <param name="resolution">downstream access token 解析結果。</param>
    internal static void ApplyAuthorization(
        HttpRequestMessage proxyRequest,
        DownstreamAccessTokenResolution resolution)
    {
        if (!resolution.IsHandled)
        {
            return;
        }

        proxyRequest.Headers.Authorization =
            resolution.AccessToken is null
                ? null
                : new AuthenticationHeaderValue("Bearer", resolution.AccessToken);
    }
}