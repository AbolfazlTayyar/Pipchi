using Ardalis.GuardClauses;
using Pipchi.Core.Enums;
using Pipchi.Core.Exceptions;
using Pipchi.Core.ValueObjects;
using Pipchi.SharedKernel;

namespace Pipchi.Core.AccountAggregate;

public class Position : BaseEntity<Guid>
{
    public Position(Guid id,
        Guid orderId,
        int symbolId,
        Volume volume,
        TradeType tradeType,
        decimal entryPrice,
        decimal? stopLoss = null,
        decimal? takeProfit = null,
        decimal? profit = null)
    {
        Id = Guard.Against.Default(id, nameof(id));
        OrderId = Guard.Against.Default(orderId, nameof(orderId));
        SymbolId = Guard.Against.Default(symbolId, nameof(symbolId));
        Volume = volume;
        Type = tradeType;
        Status = PositionStatus.Open;
        EntryPrice = Guard.Against.NegativeOrZero(entryPrice, nameof(entryPrice));
        StopLoss = stopLoss;
        TakeProfit = takeProfit;
        Profit = profit;
    }

    public Guid OrderId { get; private set; }
    public int SymbolId { get; private set; }
    public Volume Volume { get; private set; }
    public TradeType Type { get; private set; }
    public PositionStatus Status { get; private set; }
    public decimal EntryPrice { get; private set; }
    public decimal? StopLoss { get; private set; }
    public decimal? TakeProfit { get; private set; }
    public decimal? Profit { get; private set; }
    public DateTimeOffset ClosedAt { get; private set; }

    public void ClosePosition(decimal profit)
    {
        Guard.Against.NegativeOrZero(profit, nameof(profit));

        Status = PositionStatus.Closed;
        ClosedAt = DateTimeOffset.UtcNow;
        Profit = profit;

        // Add domain event PositionClosedEvent if needed
    }

    public void UpdateStopLoss(decimal stopLoss)
    {
        Guard.Against.NegativeOrZero(stopLoss, nameof(stopLoss));

        if (Type == TradeType.Buy && stopLoss >= EntryPrice)
            throw new InvalidPositionPriceRangeException("For Buy positions, StopLoss must be below EntryPrice");
        else if (Type == TradeType.Sell && stopLoss <= EntryPrice)
            throw new InvalidPositionPriceRangeException("For Sell positions, StopLoss must be above EntryPrice");
        
        StopLoss = stopLoss;

        MarkAsUpdated();

        // Add domain event PositionStopLossUpdatedEvent if needed
    }

    public void UpdateTakeProfit(decimal takeProfit)
    {
        Guard.Against.NegativeOrZero(takeProfit, nameof(takeProfit));

        if (Type == TradeType.Buy && takeProfit <= EntryPrice)
            throw new InvalidPositionPriceRangeException("For Buy positions, TakeProfit must be above EntryPrice");
        else if (Type == TradeType.Sell && takeProfit >= EntryPrice)
            throw new InvalidPositionPriceRangeException("For Sell positions, TakeProfit must be below EntryPrice");
        
        TakeProfit = takeProfit;

        MarkAsUpdated();

        // Add domain event PositionTakeProfitUpdatedEvent if needed
    }
}