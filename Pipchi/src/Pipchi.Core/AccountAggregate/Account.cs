using Ardalis.GuardClauses;
using Pipchi.Core.Enums;
using Pipchi.Core.Events;
using Pipchi.Core.Exceptions.Account;
using Pipchi.Core.Exceptions.Position;
using Pipchi.Core.Exceptions.Symbol;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.ValueObjects;
using Pipchi.SharedKernel;
using Pipchi.SharedKernel.Extensions;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Core.AccountAggregate;

public class Account : BaseEntity<Guid>, IAggregateRoot
{
    public Account(Guid id,
        Money balance,
        int leverage)
    {
        Id = Guard.Against.Default(id, nameof(id));
        Balance = balance;
        Leverage = leverage;
    }

    private Account() { } // For EF Core

    private readonly List<Order> _orders = new();
    public IEnumerable<Order> Orders => _orders.AsReadOnly();

    private readonly List<Position> _positions = new();
    public IEnumerable<Position> Positions => _positions.AsReadOnly();

    public Money Balance { get; private set; }
    public int Leverage { get; private set; }

    public Order PlaceOrder(Symbol symbol,
        TradeType type,
        decimal orderVolume,
        decimal entryPrice,
        DateTimeOffset now,
        decimal? stopLoss = null,
        decimal? takeProfit = null)
    {
        var volume = new Volume(orderVolume);

        symbol.ValidatePrice(entryPrice, nameof(entryPrice));
        symbol.ValidateVolume(volume.Value);
        symbol.EnsureMarketOpen(now);

        if (stopLoss.HasValue && stopLoss.Value > 0)
            symbol.ValidatePrice(stopLoss.Value, nameof(stopLoss));

        if (takeProfit.HasValue && takeProfit.Value > 0)
            symbol.ValidatePrice(takeProfit.Value, nameof(takeProfit));

        EnsureFreeMarginEnough(orderVolume, symbol.ContractSize, entryPrice);

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
            symbol.ContractSize,
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
        int contractSize,
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
            contractSize,
            entryPrice,
            stopLoss,
            takeProfit);

        _positions.Add(position);

        MarkAsUpdated();

        return position;
    }

    public void UpdatePosition(Guid positionId,
        decimal? stopLoss = null,
        decimal? takeProfit = null)
    {
        var position = _positions.FirstOrDefault(p => p.Id == positionId);
        if (position == null)
            throw new PositionNotFoundException($"Position with id {positionId} not found");

        if (stopLoss.HasValue)
        {
            position.UpdateStopLoss(stopLoss.Value);
            MarkAsUpdated();
        }

        if (takeProfit.HasValue)
        {
            position.UpdateTakeProfit(takeProfit.Value);
            MarkAsUpdated();
        }
    }

    public void ClosePosition(Guid positionId,
        Symbol symbol)
    {
        var position = _positions.FirstOrDefault(p => p.Id == positionId);
        if (position == null)
            throw new PositionNotFoundException($"Position with id {positionId} not found");

        position.Close(symbol);

        ApplyProfitLoss(position.Profit!.Value);

        MarkAsUpdated();
    }

    public void ClosePositions(IReadOnlyDictionary<int, Symbol> symbols)
    {
        foreach (var position in _positions.Where(p => p.Status == PositionStatus.Open))
        {
            if (!symbols.TryGetValue(position.SymbolId, out var symbol))
                throw new SymbolNotFoundException($"Symbol {position.SymbolId} not found");

            position.Close(symbol);

            ApplyProfitLoss(position.Profit!.Value);
        }

        MarkAsUpdated();
    }

    private void ApplyProfitLoss(decimal profitLoss)
    {
        Balance = new Money(Balance.Amount + profitLoss, Balance.Currency);
    }

    private void EnsureFreeMarginEnough(decimal orderVolume,
        int contractSize,
        decimal entryPrice)
    {
        var freeMargin = CalculateFreeMargin();
        var requiredMargin = CalculateRequiredMargin(orderVolume, contractSize, entryPrice);

        if (freeMargin.Amount < requiredMargin.Amount)
            throw new InsufficientMarginException($"Insufficient free margin to open this position. Required Margin: {requiredMargin.Amount.Trimmed()}, Available Margin: {freeMargin.Amount.Trimmed()}.");
    }

    private Money CalculateFreeMargin()
    {
        var equity = CalculateEquity();
        var margin = CalculateMargin();

        return new Money(equity.Amount - margin.Amount, Balance.Currency);
    }

    private Money CalculateEquity()
    {
        var floatingPnL = _positions.Where(p => p.Status == PositionStatus.Open)
            .Sum(p => p.Profit ?? 0m);

        return new Money(Balance.Amount + floatingPnL, Balance.Currency);
    }

    private Money CalculateMargin()
    {
        decimal totalMargin = 0;

        foreach (var position in _positions.Where(p => p.Status == PositionStatus.Open))
        {
            var margin = (position.Volume.Value * position.ContractSize * position.EntryPrice) / Leverage;

            totalMargin += margin;
        }

        return new Money(totalMargin, Balance.Currency);
    }

    private Money CalculateRequiredMargin(decimal orderVolume,
        int contractSize,
        decimal entryPrice)
    {
        var margin = (orderVolume * contractSize * entryPrice) / Leverage;

        return new Money(margin, Balance.Currency);
    }
}