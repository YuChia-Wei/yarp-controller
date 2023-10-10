using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Yarp.ControlPlant.WebApi.Infrastructure.Options;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>
/// This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider" /> service has been resolved from the service container.
/// via.
/// https://github.com/dotnet/aspnet-api-versioning/blob/main/examples/AspNetCore/WebApi/OpenApiExample/ConfigureSwaggerOptions.cs
/// </remarks>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions" /> class.
    /// </summary>
    /// <param name="provider">
    /// The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger
    /// documents.
    /// </param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        this._provider = provider;
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in this._provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    /// <summary>
    /// 產生對應 api version 的 swagger 資料
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = new StringBuilder($"An {AppDomain.CurrentDomain.FriendlyName} application with OpenAPI, Swashbuckle, and API versioning.");
        var info = new OpenApiInfo()
        {
            Title = $"{AppDomain.CurrentDomain.FriendlyName}",
            Version = description.ApiVersion.ToString(),
            // 可以評估撰寫 Api (系統) 負責人
            Contact = new OpenApiContact()
            {
                Name = "",
                Email = ""
            }
            // 內部系統應該沒有授權憑證問題，不過留著設定方法
            // License = new OpenApiLicense()
            // {
            //     Name = "MIT",
            //     Url = new Uri("https://opensource.org/licenses/MIT")
            // }
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        if (description.SunsetPolicy is SunsetPolicy policy)
        {
            if (policy.Date is DateTimeOffset when)
            {
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                for (var i = 0; i < policy.Links.Count; i++)
                {
                    var link = policy.Links[i];

                    if (link.Type == "text/html")
                    {
                        text.AppendLine();

                        if (link.Title.HasValue)
                        {
                            text.Append(link.Title.Value).Append(": ");
                        }

                        text.Append(link.LinkTarget.OriginalString);
                    }
                }
            }
        }

        info.Description = text.ToString();

        return info;
    }
}