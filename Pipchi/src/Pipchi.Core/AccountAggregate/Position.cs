using Ardalis.GuardClauses;
using Pipchi.Core.Enums;
using Pipchi.Core.Exceptions.Position;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.ValueObjects;
using Pipchi.SharedKernel;

namespace Pipchi.Core.AccountAggregate;

public class Position : BaseEntity<Guid>
{
    private static readonly Random _random = new();

    public Position(Guid id,
        Guid orderId,
        int symbolId,
        Volume volume,
        TradeType tradeType,
        int contractSize,
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
        ContractSize = Guard.Against.NegativeOrZero(contractSize, nameof(contractSize));
        Status = PositionStatus.Open;
        EntryPrice = Guard.Against.NegativeOrZero(entryPrice, nameof(entryPrice));
        StopLoss = stopLoss;
        TakeProfit = takeProfit;
        Profit = profit;
    }

    private Position() { } // For EF Core

    public Guid OrderId { get; private set; }
    public int SymbolId { get; private set; }
    public Volume Volume { get; private set; }
    public TradeType Type { get; private set; }
    public PositionStatus Status { get; private set; }
    public int ContractSize { get; private set; }
    public decimal EntryPrice { get; private set; }
    public decimal? StopLoss { get; private set; }
    public decimal? TakeProfit { get; private set; }
    public decimal? Profit { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }

    public void Close(Symbol symbol)
    {
        EnsureNotClosed();

        symbol.EnsureMarketOpen(DateTimeOffset.UtcNow);

        Profit = CalculateProfitLoss();
        Status = PositionStatus.Closed;
        ClosedAt = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void UpdateStopLoss(decimal stopLoss)
    {
        EnsureNotClosed();

        Guard.Against.Negative(stopLoss, nameof(stopLoss));

        if (stopLoss != 0)
        {
            if (Type == TradeType.Buy && stopLoss >= EntryPrice)
                throw new InvalidPositionPriceRangeException("For Buy positions, StopLoss must be below EntryPrice");
            else if (Type == TradeType.Sell && stopLoss <= EntryPrice)
                throw new InvalidPositionPriceRangeException("For Sell positions, StopLoss must be above EntryPrice");

            StopLoss = stopLoss;

            MarkAsUpdated();

            // Add domain event PositionStopLossUpdatedEvent if needed
        }
    }

    public void UpdateTakeProfit(decimal takeProfit)
    {
        EnsureNotClosed();

        Guard.Against.Negative(takeProfit, nameof(takeProfit));

        if (takeProfit != 0)
        {
            if (Type == TradeType.Buy && takeProfit <= EntryPrice)
                throw new InvalidPositionPriceRangeException("For Buy positions, TakeProfit must be above EntryPrice");
            else if (Type == TradeType.Sell && takeProfit >= EntryPrice)
                throw new InvalidPositionPriceRangeException("For Sell positions, TakeProfit must be below EntryPrice");

            TakeProfit = takeProfit;

            MarkAsUpdated();

            // Add domain event PositionTakeProfitUpdatedEvent if needed
        }
    }

    private decimal CalculateProfitLoss()
    {
        var percentChange = (decimal)(_random.NextDouble() * 0.1 - 0.05);
        var priceChange = EntryPrice * percentChange;
        var profitLoss = priceChange * Volume.Value * 100000m;

        return Math.Round(Type == TradeType.Sell ? -profitLoss : profitLoss, 2);
    }

    private void EnsureNotClosed()
    {
        if (Status == PositionStatus.Closed)
            throw new PositionClosedException($"Position with id {Id} is closed and this operation cannot be performed.");
    }
}