namespace Pipchi.Api.Models.Symbol;

public class CreateSymbolRequest : BaseRequest
{
    public string Name { get; set; }
    public int Digits { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal MinVolume { get; set; }
    public decimal MaxVolume { get; set; }
}
