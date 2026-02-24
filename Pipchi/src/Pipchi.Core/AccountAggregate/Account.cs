using Ardalis.GuardClauses;
using MediatR;
using Pipchi.Core.Enums;
using Pipchi.Core.Events;
using Pipchi.Core.Exceptions.Account;
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
        decimal orderVolume,
        decimal entryPrice,
        decimal? stopLoss = null,
        decimal? takeProfit = null)
    {
        var volume = new Volume(orderVolume);

        symbol.ValidatePrice(entryPrice);
        symbol.ValidateVolume(volume.Value);
        symbol.EnsureMarketOpen(DateTimeOffset.UtcNow);

        if (stopLoss.HasValue && stopLoss.Value > 0)
            symbol.ValidatePrice(stopLoss.Value);

        if (takeProfit.HasValue && takeProfit.Value > 0)
            symbol.ValidatePrice(takeProfit.Value);

        var order = new Order(Guid.NewGuid(),
            Id,
            symbol.Id,
            type,
            volume,
            entryPrice,
            stopLoss,
            takeProfit);

        _orders.Add(order);

        Events.Add(new OrderPlacedEvent(order.Id,
            order.AccountId,
            order.SymbolId,
            order.Volume.Value,
            order.Type,
            order.EntryPrice,
            order.StopLoss,
            order.TakeProfit));

        MarkAsUpdated();

        return order;
    }

    public Position AddPosition(Guid orderId,
        int symbolId,
        decimal orderVolume,
        TradeType type,
        decimal entryPrice,
        decimal? stopLoss = null,
        decimal? takeProfit = null)
    {
        var volume = new Volume(orderVolume);

        var position = new Position(Guid.NewGuid(),
            orderId,
            symbolId,
            volume,
            type,
            entryPrice,
            stopLoss,
            takeProfit);

        _positions.Add(position);

        MarkAsUpdated();

        return position;
    }
}