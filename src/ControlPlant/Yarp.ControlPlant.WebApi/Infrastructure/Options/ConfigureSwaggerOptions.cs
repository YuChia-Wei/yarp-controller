using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Yarp.ControlPlant.WebApi.Infrastructure.Options;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions" /> class.
    /// </summary>
    public ConfigureSwaggerOptions()
    {
    }

    /// <inheritdoc />
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        options.SwaggerDoc("default version", CreateInfoForApiVersion());
    }

    /// <summary>
    /// 產生對應 api version 的 swagger 資料
    /// </summary>
    /// <returns></returns>
    private static OpenApiInfo CreateInfoForApiVersion()
    {
        var text = new StringBuilder($"An {AppDomain.CurrentDomain.FriendlyName} application with OpenAPI, Swashbuckle, and API versioning.");
        var info = new OpenApiInfo()
        {
            Title = $"{AppDomain.CurrentDomain.FriendlyName}",
            Version = "v1",
            // 可以評估撰寫 Api (系統) 負責人
            Contact = new OpenApiContact()
            {
                Name = "",
                Email = ""
            }
            // define license
            // License = new OpenApiLicense()
            // {
            //     Name = "MIT",
            //     Url = new Uri("https://opensource.org/licenses/MIT")
            // }
        };

        info.Description = text.ToString();

        return info;
    }
}