using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Pipchi.Api.Abstractions;
using Pipchi.Api.Extensions;
using Pipchi.Api.Models.Symbol;

namespace Pipchi.Api.Endpoints.v1.Symbol;

public class Create : IEndpointDefinition
{
    public void RegisterEndpoint(WebApplication app, ApiVersionSet apiVersionSet)
    {
        var symbol = app.CreateVersionedGroup("symbols", apiVersionSet)
            .WithTags("Symbol");

        symbol.MapPost("/", async ([FromBody] CreateSymbolRequest request, CancellationToken cancellationToken) =>
        {
            return Results.Ok();
        }).WithSummary("Creates a new Symbol");
    }
}
