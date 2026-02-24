using Pipchi.Core.Enums;
using Pipchi.SharedKernel;

namespace Pipchi.Core.Events;

public class OrderPlacedEvent : BaseDomainEvent
{
    public Guid OrderId { get; }
    public Guid AccountId { get; }
    public int SymbolId { get; }
    public decimal Volume { get; }
    public TradeType Type { get; }
    public decimal EntryPrice { get; }
    public decimal? StopLoss { get; }
    public decimal? TakeProfit { get; }

    public OrderPlacedEvent(Guid orderId,
        Guid accountId,
        int symbolId,
        decimal volume,
        TradeType type,
        decimal entryPrice,
        decimal? stopLoss,
        decimal? takeProfit)
    {
        OrderId = orderId;
        AccountId = accountId;
        SymbolId = symbolId;
        Volume = volume;
        Type = type;
        EntryPrice = entryPrice;
        StopLoss = stopLoss;
        TakeProfit = takeProfit;
    }
}
