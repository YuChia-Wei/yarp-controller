using Asp.Versioning;
using Asp.Versioning.Conventions;
using Yarp.ControlPlant.UseCase.Port.Input.Cluster;
using Yarp.ControlPlant.UseCase.Port.Input.Route;
using Yarp.ControlPlant.UseCase.Port.Output.Cluster;
using Yarp.ControlPlant.UseCase.Port.Output.Route;
using Yarp.ControlPlant.WebApi.Infrastructure.BuilderExtension;
using Yarp.ControlPlant.WebApi.Infrastructure.Options;
using Yarp.ControlPlant.WebApi.Infrastructure.ServiceCollectionExtension;

var builder = WebApplication.CreateBuilder(args);

var authOptions = AuthOptions.CreateInstance(builder.Configuration);

builder.Services.AddScoped<IRouteCreator, RouteConfigJsonCreator>();
builder.Services.AddScoped<IClusterCreator, ClusterConfigJsonCreator>();
builder.Services.AddScoped<IRouteExistChecker, RouteExistChecker>();
builder.Services.AddScoped<IClusterExistChecker, ClusterExistChecker>();

builder.Services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped);

builder.Services.AddHealthChecks();

builder.Services.AddApiVersioning(options =>
       {
           options.ReportApiVersions = true;
           options.AssumeDefaultVersionWhenUnspecified = true;
           options.DefaultApiVersion = new ApiVersion(1, 0);
       })
       .AddMvc(options => options.Conventions.Add(new VersionByNamespaceConvention()))
       .AddApiExplorer(options =>
       {
           options.GroupNameFormat = "'v'VVV";
           options.SubstituteApiVersionInUrl = true;
       });

builder.Services.AddSwaggerSetting(authOptions);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseHealthChecks("/health");

app.UseSwaggerRoute(authOptions);

app.MapDefaultControllerRoute();

app.MapControllers();

app.Run();