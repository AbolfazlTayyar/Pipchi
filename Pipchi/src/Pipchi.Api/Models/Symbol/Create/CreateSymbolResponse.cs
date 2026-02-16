using Pipchi.Api.Models.DTOs;

namespace Pipchi.Api.Models.Symbol.Create;

public class CreateSymbolResponse : BaseResponse
{
    public CreateSymbolResponse(Guid correlationId) : base(correlationId)
    {
    }

    public SymbolDto SymbolDto { get; set; } = new();
}
