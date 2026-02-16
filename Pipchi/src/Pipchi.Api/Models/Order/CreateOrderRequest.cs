using Pipchi.Core.Enums;

namespace Pipchi.Api.Models.Order;

public class CreateOrderRequest : BaseRequest
{
    public const string Route = "api/orders";

    public Guid AccountId { get; set; }
    public int SymbolId { get; set; }
    public TradeType Type { get; set; }
    public decimal Volume { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
}
