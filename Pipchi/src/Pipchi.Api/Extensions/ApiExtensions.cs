using Asp.Versioning.Builder;
using Asp.Versioning.Conventions;
using Pipchi.Api.Abstractions;

namespace Pipchi.Api.Extensions;

public static class ApiExtensions
{
    // mayble the whole minimal api usage was a little bit overkill, but it was fun to implement and learn about it 
    public static void RegisterEndpoints(this WebApplication app, ApiVersionSet apiVersionSet)
    {
        var endpoints = typeof(Program).Assembly
            .GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IEndpointDefinition)) && !x.IsAbstract && !x.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IEndpointDefinition>();

        foreach (var endpoint in endpoints)
        {
            endpoint.RegisterEndpoint(app, apiVersionSet);
        }
    }

    public static ApiVersionSet ConfigureApiVersioning(this WebApplication app)
    {
        return app.NewApiVersionSet()
            .HasApiVersion(1.0)
            .ReportApiVersions()
            .Build();
    }

    public static RouteGroupBuilder CreateVersionedGroup(this WebApplication app, string path,
        ApiVersionSet apiVersionSet, double version = 1.0)
    {
        return app.MapGroup($"/api/v{{version:apiVersion}}/{path}")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(version);
    }
}
