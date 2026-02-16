using Ardalis.GuardClauses;
using Pipchi.Core.Enums;
using Pipchi.Core.Interfaces;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.ValueObjects;
using Pipchi.SharedKernel;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Core.AccountAggregate;

public class Account : BaseEntity<Guid>, IAggregateRoot
{
    public Account(Guid id,
        Money balance)
    {
        Id = Guard.Against.Default(id, nameof(id));
        Balance = balance;
    }

    private Account() { } // For EF Core

    private readonly List<Order> _orders = new();
    public IEnumerable<Order> Orders => _orders.AsReadOnly();

    private readonly List<Position> _positions = new();
    public IEnumerable<Position> Positions => _positions.AsReadOnly();

    public Money Balance { get; private set; }

    public Money CalculateEquity(IEquityCalculator calculator)
        => calculator.Calculate(this);

    public Money CalculateMargin(IMarginCalculator calculator)
        => calculator.Calculate(this);

    public Money CalculateFreeMargin(IEquityCalculator equityCalculator,
                                     IMarginCalculator marginCalculator)
    {
        var equity = CalculateEquity(equityCalculator);
        var margin = CalculateMargin(marginCalculator);
        return equity - margin;
    }

    public Order PlaceOrder(Symbol symbol,
        TradeType type,
        Volume volume,
        decimal entryPrice,
        decimal? stopLoss = null,
        decimal? takeProfit = null)
    {
        symbol.ValidatePrice(entryPrice);
        symbol.ValidateVolume(volume.Value);

        if (stopLoss.HasValue)
            symbol.ValidatePrice(stopLoss.Value);

        if (takeProfit.HasValue)
            symbol.ValidatePrice(takeProfit.Value);

        var order = new Order(
            Guid.NewGuid(),
            Id,
            symbol.Id,
            type,
            volume,
            entryPrice,
            stopLoss,
            takeProfit);

        _orders.Add(order);

        //AddDomainEvent(new OrderPlacedEvent(Id, order.Id, symbolId, volume.Value));

        MarkAsUpdated();

        return order;
    }
}