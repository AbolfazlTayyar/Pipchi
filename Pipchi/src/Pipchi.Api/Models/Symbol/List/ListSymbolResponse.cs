namespace Pipchi.Api.Models.Symbol.List;

public class ListSymbolResponse : BaseResponse
{
    public ListSymbolResponse(Guid correlationId) : base(correlationId)
    {
    }

    public List<SymbolDto> Symbols { get; set; } = new();
}
