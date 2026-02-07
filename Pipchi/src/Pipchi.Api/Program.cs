using Pipchi.Api;
using Pipchi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationsIfPending();
}

app.UseHttpsRedirection();

app.Run();
