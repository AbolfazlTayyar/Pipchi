using Pipchi.Core.AccountAggregate;
using Pipchi.Core.Interfaces;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Infrastructure.Implementations;

public class FreeMarginCalculator : IFreeMarginCalculator
{
    private readonly IEquityCalculator _equityCalculator;
    private readonly IUsedMarginCalculator _marginCalculator;

    public FreeMarginCalculator(IEquityCalculator equityCalculator,
        IUsedMarginCalculator marginCalculator)
    {
        _equityCalculator = equityCalculator;
        _marginCalculator = marginCalculator;
    }

    public Money Calculate(Account account, IReadOnlyDictionary<int, Symbol> symbols)
    {
        var equity = _equityCalculator.Calculate(account);
        var usedMargin = _marginCalculator.Calculate(account, symbols);

        return new Money(equity.Amount - usedMargin.Amount, account.Balance.Currency);
    }
}