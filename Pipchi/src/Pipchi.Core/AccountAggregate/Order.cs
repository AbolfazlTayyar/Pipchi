using Ardalis.GuardClauses;
using Pipchi.Core.Enums;
using Pipchi.Core.Exceptions;
using Pipchi.Core.ValueObjects;
using Pipchi.SharedKernel;

namespace Pipchi.Core.AccountAggregate;

public class Order : BaseEntity<Guid>
{
    public Order(Guid id,
        Guid accountId,
        int symbolId,
        TradeType orderType,
        Volume volume,
        decimal entryPrice,
        decimal? stopLoss = null,
        decimal? takeProfit = null)
    {
        Id = Guard.Against.Default(id, nameof(id));
        AccountId = Guard.Against.Default(accountId, nameof(accountId));
        SymbolId = Guard.Against.Default(symbolId, nameof(symbolId));
        Type = orderType;
        EntryPrice = Guard.Against.NegativeOrZero(entryPrice, nameof(entryPrice));
        Volume = volume;
        StopLoss = stopLoss;
        TakeProfit = takeProfit;
    }

    public Guid AccountId { get; private set; }
    public int SymbolId { get; private set; }
    public TradeType Type { get; private set; }
    public Volume Volume { get; private set; }
    public decimal EntryPrice { get; private set; }
    public decimal? StopLoss { get; private set; }
    public decimal? TakeProfit { get; private set; }

    private void ValidateStopLossAndTakeProfit()
    {
        if (Type == TradeType.Buy)
        {
            if (StopLoss.HasValue && StopLoss >= EntryPrice)
                throw new InvalidOrderPriceRangeException("For Buy orders, StopLoss must be below EntryPrice");

            if (TakeProfit.HasValue && TakeProfit <= EntryPrice)
                throw new InvalidOrderPriceRangeException("For Buy orders, TakeProfit must be above EntryPrice");

            if (StopLoss.HasValue && TakeProfit.HasValue && StopLoss >= TakeProfit)
                throw new InvalidOrderPriceRangeException("For Buy orders, StopLoss must be below TakeProfit");
        }
        else // Sell
        {
            if (StopLoss.HasValue && StopLoss <= EntryPrice)
                throw new InvalidOrderPriceRangeException("For Sell orders, StopLoss must be above EntryPrice");

            if (TakeProfit.HasValue && TakeProfit >= EntryPrice)
                throw new InvalidOrderPriceRangeException("For Sell orders, TakeProfit must be below EntryPrice");

            if (StopLoss.HasValue && TakeProfit.HasValue && StopLoss <= TakeProfit)
                throw new InvalidOrderPriceRangeException("For Sell orders, StopLoss must be above TakeProfit");
        }
    }
}
