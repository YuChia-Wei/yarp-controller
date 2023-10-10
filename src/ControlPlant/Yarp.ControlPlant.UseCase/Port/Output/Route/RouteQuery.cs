using System.Text.Json;
using System.Text.Json.Serialization;
using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Port.Output.Route;

public class RouteQuery : IRouteQuery
{
    public async Task<RouteConfig?> QueryAsync(string routeId)
    {
        var combine = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.json");
        if (!File.Exists(combine))
        {
            return default;
        }

        var readAllTextAsync = await File.ReadAllTextAsync(combine);

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var serialize = JsonSerializer.Deserialize<RouteConfig>(readAllTextAsync, jsonSerializerOptions);

        return serialize;
    }
}