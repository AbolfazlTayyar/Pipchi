namespace Pipchi.Api.Models.Symbol.Create;

public class CreateSymbolRequest : BaseRequest
{
    public const string Route = "api/symbols";

    public string Name { get; set; }
    public int Digits { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public decimal MinVolume { get; set; }
    public decimal MaxVolume { get; set; }
    public decimal VolumeStep { get; set; }
    public int ContractSize { get; set; }
    public TimeOnly? MarketOpenTime { get; set; }
    public TimeOnly? MarketCloseTime { get; set; }
}
