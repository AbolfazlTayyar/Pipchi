using Pipchi.Api;
using Pipchi.Api.Extensions;
using Pipchi.Infrastructure;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApiVersioning();

builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsIfPending();
}

app.UseSwaggerDocumentation();

app.UseHttpsRedirection();

var apiVersionSet = app.ConfigureApiVersioning();
app.RegisterEndpoints(apiVersionSet);

app.Run();
