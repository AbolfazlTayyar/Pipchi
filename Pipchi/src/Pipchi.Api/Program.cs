using FastEndpoints;
using FastEndpoints.Swagger;
using Pipchi.Api;
using Pipchi.Api.Extensions;
using Pipchi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerDocumentation();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsIfPending();
}

app.UseSwaggerUiRedirect();

app.UseHttpsRedirection();

app.UseFastEndpoints().UseSwaggerGen();

app.Run();
