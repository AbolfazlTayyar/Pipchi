using Ardalis.GuardClauses;
using Pipchi.Core.Enums;
using Pipchi.Core.Exceptions;

namespace Pipchi.Core.AccountAggregate.Guards;

public static class OrderGuardExtensions
{
    public static decimal InvalidStopLoss(this IGuardClause guardClause, TradeType orderType, decimal? stopLoss, decimal entryPrice)
    {
        if (orderType == TradeType.Buy)
        {
            if (stopLoss.HasValue && stopLoss.Value > 0 && stopLoss >= entryPrice)
                throw new InvalidOrderPriceRangeException("For Buy orders, StopLoss must be below EntryPrice");
        }
        else // Sell
        {
            if (stopLoss.HasValue && stopLoss.Value > 0 && stopLoss <= entryPrice)
                throw new InvalidOrderPriceRangeException("For Sell orders, StopLoss must be above EntryPrice");
        }

        return stopLoss.Value;
    }

    public static decimal InvalidTakeProfit(this IGuardClause guardClause, TradeType orderType, decimal? takeProfit, decimal entryPrice)
    {
        if (orderType == TradeType.Buy)
        {
            if (takeProfit.HasValue && takeProfit.Value > 0 && takeProfit <= entryPrice)
                throw new InvalidOrderPriceRangeException("For Buy orders, TakeProfit must be above EntryPrice");
        }
        else // Sell
        {
            if (takeProfit.HasValue && takeProfit.Value > 0 && takeProfit >= entryPrice)
                throw new InvalidOrderPriceRangeException("For Sell orders, TakeProfit must be below EntryPrice");
        }

        return takeProfit.Value;
    }
}
