using System.Text.Json;
using System.Text.Json.Serialization;
using Yarp.ReverseProxy.ControlPlant.Entity.Configuration;

namespace Yarp.ControlPlant.UseCase.Port.Output.Route;

public class RouteConfigJsonCreator : IRouteCreator
{
    public async Task CreateAsync(RouteConfig routeConfig)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.Strict,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var serialize = JsonSerializer.Serialize(routeConfig, jsonSerializerOptions);
        var combine = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.json");
        await File.WriteAllTextAsync(combine, serialize);
    }
}