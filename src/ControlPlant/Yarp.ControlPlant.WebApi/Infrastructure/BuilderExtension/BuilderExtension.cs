using Asp.Versioning.ApiExplorer;
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
        var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
                    $"{description.GroupName}/swagger.json", $"{AppDomain.CurrentDomain.FriendlyName} {description.GroupName}");
            }

            options.OAuthClientId(authOptions.ClientId);
            options.OAuthClientSecret(authOptions.ClientSecret);
            options.OAuthScopeSeparator(" ");
            options.OAuthUsePkce();
        });

        app.UseReDoc(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SpecUrl($"/swagger/{description.GroupName}/swagger.json");
                options.RoutePrefix = $"redoc";
                options.DocumentTitle = $"{AppDomain.CurrentDomain.FriendlyName} {description.GroupName}";
            }
        });
    }
}