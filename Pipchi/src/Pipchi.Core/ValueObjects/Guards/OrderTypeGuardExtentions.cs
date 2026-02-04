using Ardalis.GuardClauses;

namespace Pipchi.Core.ValueObjects.Guards;

public static class OrderTypeGuardExtentions
{
    public static OrderType AgainstInvalidOrderType(this IGuardClause guardClause, OrderType orderType, string parameterName)
    {
        if (orderType != OrderType.Buy && orderType != OrderType.Sell)
            throw new ArgumentException($"Invalid order type: {orderType?.Name}", parameterName);

        return orderType;
    }
}