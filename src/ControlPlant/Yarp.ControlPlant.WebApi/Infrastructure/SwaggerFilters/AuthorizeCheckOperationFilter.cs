using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Yarp.ControlPlant.WebApi.Infrastructure.SwaggerFilters;

/// <summary>
/// Swagger 使用的 AuthorizeCheckOperationFilter
/// </summary>
/// <seealso cref="IOperationFilter" />
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the specified operation.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="context">The context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize =
            context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
            context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

        if (hasAuthorize)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "OAuth2"
                            }
                        }
                    ] = new string[0] //這邊可以設定 Token 應該要符合哪些 Scope 才在 API 右側顯示鎖頭
                }
                //TODO: 若需要啟用 ClientCredentials 的 OAuth 認證流程，請反註解此段
                //new OpenApiSecurityRequirement
                //{
                //    [
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme, Id = "Internal"
                //            }
                //        }
                //    ] = new string[0] //這邊可以設定 Token 應該要符合哪些 Scope 才在 API 右側顯示鎖頭
                //}
            };
        }
    }
}