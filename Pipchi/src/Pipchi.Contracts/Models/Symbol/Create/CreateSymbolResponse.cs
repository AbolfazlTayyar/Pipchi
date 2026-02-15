using Pipchi.Contracts.DTOs;

namespace Pipchi.Contracts.Models.Symbol.Create;

public class CreateSymbolResponse : BaseResponse
{
    public CreateSymbolResponse(Guid correlationId) : base(correlationId)
    {
    }

    public SymbolDto SymbolDto { get; set; } = new SymbolDto();
}
