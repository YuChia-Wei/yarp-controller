using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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
        if (transformContext.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            var tokenAsync = await transformContext.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (!string.IsNullOrEmpty(tokenAsync))
            {
                transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenAsync);
            }
        }
    }
}