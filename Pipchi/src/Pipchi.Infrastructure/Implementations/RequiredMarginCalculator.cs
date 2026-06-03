using Pipchi.Core.AccountAggregate;
using Pipchi.Core.Interfaces;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Infrastructure.Implementations;

public class RequiredMarginCalculator : IRequiredMarginCalculator
{
    public Money Calculate(Account account, Symbol symbol, decimal volume, decimal price)
    {
        var margin = (volume * symbol.ContractSize * price) / account.Leverage;

        return new Money(margin, account.Balance.Currency);
    }
}
