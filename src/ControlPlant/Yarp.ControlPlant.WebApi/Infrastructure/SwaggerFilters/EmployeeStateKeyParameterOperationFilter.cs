using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Yarp.ControlPlant.WebApi.Infrastructure.SwaggerFilters;

/// <summary>
/// 在 Swagger 文件中的每條 API 上加入 Employee State Key 的 Header 參數
/// </summary>
/// <seealso cref="Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter" />
public class EmployeeStateKeyParameterOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the specified operation.
    /// </summary>
    /// <param name="operation">The operation.</param>
    /// <param name="context">The context.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "StateKey",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Employee State Key",
            Schema = new OpenApiSchema { Type = "string" }
        });
    }
}