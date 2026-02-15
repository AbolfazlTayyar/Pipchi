using FastEndpoints;
using FastEndpoints.Swagger;

namespace Pipchi.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services
            .AddFastEndpoints()
            .SwaggerDocument(options =>
            {
                options.DocumentSettings = s =>
                {
                    s.Title = "My API V1";
                };
            });

        return services;
    }

    public static IApplicationBuilder UseSwaggerUiRedirect(this WebApplication app)
    {
        // Redirect root to Swagger
        app.MapGet("/", () => Results.Redirect("/swagger"))
            .ExcludeFromDescription();

        return app;
    }
}
