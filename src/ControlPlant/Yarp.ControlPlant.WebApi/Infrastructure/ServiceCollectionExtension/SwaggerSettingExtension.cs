using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ControlPlant.WebApi.Infrastructure.Options;
using Yarp.ControlPlant.WebApi.Infrastructure.SwaggerFilters;

namespace Yarp.ControlPlant.WebApi.Infrastructure.ServiceCollectionExtension;

/// <summary>
/// Swagger 設定的擴充方法
/// </summary>
public static class SwaggerSettingExtension
{
    /// <summary>
    /// Adds the swagger setting.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="authOptions">The authentication options.</param>
    public static void AddSwaggerSetting(this IServiceCollection services, AuthOptions authOptions)
    {
        // 取得所有 Swagger Examples
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(options =>
        {
            //Swagger OAuth Setting
            options.AddSecurityDefinition(
                "OAuth2",
                new OpenApiSecurityScheme
                {
                    Description = $@"模擬 Authorization Code, 請先勾選 scope: {AppDomain.CurrentDomain.FriendlyName} API",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl =
                                new Uri($"{authOptions.Authority}/connect/authorize"),
                            TokenUrl = new Uri($"{authOptions.Authority}/connect/token"),
                            Scopes = new Dictionary<string, string> { { authOptions.Audience, $"{AppDomain.CurrentDomain.FriendlyName} API" } }
                        }
                    }
                });

            //TODO: 若需要在 swagger 上啟用 ClientCredentials 的 OAuth 認證流程，請反註解此段
            //options.AddSecurityDefinition(
            //    "Internal",
            //    new OpenApiSecurityScheme
            //    {
            //        Description =
            //            @"內部 API 呼叫的認證\n" +
            //            "此 OAuth Flow 採用 ClientCredentials，Client Id / Secret 與預設不同，需要額外輸入",
            //        Type = SecuritySchemeType.OAuth2,
            //        Flows = new OpenApiOAuthFlows
            //        {
            //            ClientCredentials = new OpenApiOAuthFlow
            //            {
            //                AuthorizationUrl = new Uri($"{authOptions.Authority}/connect/authorize"),
            //                TokenUrl = new Uri($"{authOptions.Authority}/connect/token"),
            //                Scopes = new Dictionary<string, string>
            //                {
            //                    { authOptions.Audience, $"{AppDomain.CurrentDomain.FriendlyName} API" }
            //                }
            //            }
            //        }
            //    });

            // 掛載 ExampleFilter
            options.ExampleFilters();

            //Add Swagger OAuth Setting 
            options.OperationFilter<AuthorizeCheckOperationFilter>();

            //Add Custom Header
            options.OperationFilter<EmployeeStateKeyParameterOperationFilter>();

            #region Use WebService Controller / Parameter / ViewModel Comment

            // var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
            var fileName = AppDomain.CurrentDomain.FriendlyName + ".xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            // integrate xml comments
            options.IncludeXmlComments(filePath);

            #endregion

            //TODO: 如果有需要暴露 WebService 以外的專案中的物件說明再使用此區塊的作法設定 Swagger 文件，一般來說不建議

            #region Use All XML In Project

            // var basePath = AppContext.BaseDirectory;
            // var xmlFiles = Directory.EnumerateFiles(basePath, "*.xml", SearchOption.TopDirectoryOnly);
            //
            // foreach (var xmlFile in xmlFiles)
            // {
            //     options.IncludeXmlComments(xmlFile);
            // }

            #endregion
        });
    }
}