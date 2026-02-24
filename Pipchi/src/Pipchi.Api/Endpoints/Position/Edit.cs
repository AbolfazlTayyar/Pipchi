using FastEndpoints;
using Pipchi.Api.Models.Position;

namespace Pipchi.Api.Endpoints.Position;

public class Edit : Endpoint<EditPositionRequest, EditPositionResponse>
{
    public override void Configure()
    {
        Put(EditPositionRequest.Route);
        AllowAnonymous();
        Description(x =>
            x.WithSummary("Updates a Position")
                .WithDescription("Updates a Position")
                .WithName("positions.update")
                .WithTags("PositionEndpoints"));
    }

    public override Task<EditPositionResponse> ExecuteAsync(EditPositionRequest req, CancellationToken ct)
    {
        return base.ExecuteAsync(req, ct);
    }
}
