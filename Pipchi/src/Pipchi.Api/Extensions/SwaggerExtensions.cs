using Microsoft.OpenApi;

namespace Pipchi.Api.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Pipchi Trading Api",
                Description = "A trading engine Api built with DDD principles.",
                Contact = new OpenApiContact
                {
                    Name = "Abolfazl Tayyar",
                    Email = "AbolfazlTayyar@gmail.com",
                    Url = new Uri("https://github.com/AbolfazlTayyar/Pipchi"),
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // XML Comments (optional but recommended)
            //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //if (File.Exists(xmlPath))
            //{
            //    options.IncludeXmlComments(xmlPath);
            //}

            // JWT Authentication (for future)
            //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //{
            //    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
            //    Name = "Authorization",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "Bearer"
            //});

            //options.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    {
            //        new OpenApiSecurityScheme
            //        {
            //            Reference = new OpenApiReference
            //            {
            //                Type = ReferenceType.SecurityScheme,
            //                Id = "Bearer"
            //            }
            //        },
            //        Array.Empty<string>()
            //    }
            //});

            //// Custom operation IDs
            //options.CustomOperationIds(apiDesc =>
            //{
            //    return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
            //        ? methodInfo.Name
            //        : null;
            //});

            //// Order actions by method
            //options.OrderActionsBy(apiDesc =>
            //    $"{apiDesc.RelativePath}_{apiDesc.HttpMethod}");
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI();

            // Redirect root to Swagger
            app.MapGet("/", () => Results.Redirect("/swagger"))
                .ExcludeFromDescription();
        }

        return app;
    }
}
