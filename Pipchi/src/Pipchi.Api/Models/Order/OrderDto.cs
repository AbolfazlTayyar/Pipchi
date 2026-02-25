using Pipchi.Core.Enums;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Api.Models.Order;

public class OrderDto
{
    public Guid AccountId { get; set; }
    public int SymbolId { get; set; }
    public TradeType Type { get; set; }
    public Volume Volume { get; set; }
    public decimal EntryPrice { get; set; }
    public decimal? StopLoss { get; set; }
    public decimal? TakeProfit { get; set; }
    public TimeOnly MarketOpenTime { get; private set; }
    public TimeOnly MarketCloseTime { get; private set; }
}
