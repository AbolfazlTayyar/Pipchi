using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Pipchi.Core.AccountAggregate;

namespace Pipchi.Api.Models.Position;

public class EditPositionResponse : BaseResponse
{
    public EditPositionResponse(Guid correlationId) : base(correlationId)
    {
    }

    public PositionDto PositionDto { get; set; } = new();
}