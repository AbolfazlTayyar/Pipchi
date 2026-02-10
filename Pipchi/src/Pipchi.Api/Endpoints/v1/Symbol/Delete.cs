//using Asp.Versioning.Builder;
//using Microsoft.AspNetCore.Mvc;
//using Pipchi.Api.Abstractions;
//using Pipchi.Api.Extensions;
//using Pipchi.Api.Models.Symbol;

//namespace Pipchi.Api.Endpoints.v1.Symbol;

//public class Delete : IEndpointDefinition
//{
//    public void RegisterEndpoint(WebApplication app, ApiVersionSet apiVersionSet)
//    {
//        //var symbol = app.MapGroup("/api/v1/symbols")
//        //    .WithApiVersionSet(apiVersionSet)
//        //    .HasApiVersion(1.0)
//        //    .WithTags("Symbol");

//        var symbolV1 = app.CreateVersionedGroup("symbols", apiVersionSet);

//        symbolV1.MapDelete("/", async ([FromBody] DeleteSymbolRequest request, CancellationToken cancellationToken) =>
//        {
//            return Results.Ok();
//        }).WithSummary("Delete a Symbol");

//        var symbolV2 = app.CreateVersionedGroup("symbols", apiVersionSet, 2);

//        symbolV2.MapDelete("/", async () =>
//        {
//            return Results.Ok();
//        }).WithSummary("Delete a Symbol");
//    }
//}
