using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Pipchi.Api.Abstractions;
using Pipchi.Contracts.Models.Symbol.Create;

namespace Pipchi.Api.Endpoints.v1.Symbol;

public class Create : IEndpointDefinition
{
    public void RegisterEndpoint(WebApplication app, ApiVersionSet apiVersionSet)
    {
        var symbol = app.MapGroup("/api/v1/symbols");

        symbol.MapPost("/", async ([FromBody] CreateSymbolRequest request, CancellationToken cancellationToken) =>
        {
            var response = new CreateSymbolResponse(request.CorrelationId);

            var symbol = new Core.SyncedAggregates.Symbol(request.Name, request.Digits, request.MinPrice,
                request.MaxPrice, request.MinVolume, request.MaxVolume);


        }).WithSummary("Creates a new Symbol");
    }
}
