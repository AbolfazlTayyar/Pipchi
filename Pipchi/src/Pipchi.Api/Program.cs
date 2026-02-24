using FastEndpoints;
using FastEndpoints.Swagger;
using Pipchi.Api;
using Pipchi.Api.Extensions;
using Pipchi.Core.AccountAggregate;
using Pipchi.Infrastructure;
using Pipchi.Infrastructure.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSwaggerDocumentation();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

var assemblies = new Assembly[] { typeof(Program).Assembly, typeof(ApplicationDbContext).Assembly, typeof(Account).Assembly };
builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblies(assemblies));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsIfPending();
}

app.UseSwaggerUiRedirect();

app.UseHttpsRedirection();

app.UseFastEndpoints().UseSwaggerGen();

app.Run();
