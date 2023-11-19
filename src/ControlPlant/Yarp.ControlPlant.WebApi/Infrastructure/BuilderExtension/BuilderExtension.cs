using Yarp.ControlPlant.WebApi.Infrastructure.Options;

namespace Yarp.ControlPlant.WebApi.Infrastructure.BuilderExtension;

/// <summary>
/// App Builder 擴充
/// </summary>
public static class BuilderExtension
{
    /// <summary>
    /// Uses the swagger (UI / OpenApi Json Routing).
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="authOptions"></param>
    public static void UseSwaggerRoute(this IApplicationBuilder app,
                                       AuthOptions authOptions)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger.json", $"{AppDomain.CurrentDomain.FriendlyName}");
            options.OAuthClientId(authOptions.ClientId);
            options.OAuthClientSecret(authOptions.ClientSecret);
            options.OAuthScopeSeparator(" ");
            options.OAuthUsePkce();
        });

        app.UseReDoc(options =>
        {
            options.SpecUrl($"/swagger.json");
            options.RoutePrefix = $"redoc";
            options.DocumentTitle = $"{AppDomain.CurrentDomain.FriendlyName}";
        });
    }
}