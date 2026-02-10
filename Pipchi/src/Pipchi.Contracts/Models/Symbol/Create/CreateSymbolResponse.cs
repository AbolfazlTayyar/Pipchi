namespace Pipchi.Contracts.Models.Symbol.Create;

public class CreateSymbolResponse : BaseResponse
{
    public CreateSymbolResponse(Guid correlationId) : base(correlationId)
    {
    }

    public int MyProperty { get; set; }
}
