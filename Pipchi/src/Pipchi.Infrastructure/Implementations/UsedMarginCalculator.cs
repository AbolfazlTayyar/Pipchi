using Pipchi.Core.AccountAggregate;
using Pipchi.Core.Enums;
using Pipchi.Core.Interfaces;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Infrastructure.Implementations;

public class UsedMarginCalculator : IUsedMarginCalculator
{
    public Money Calculate(Account account, IReadOnlyDictionary<int, Symbol> symbols)
    {
        decimal totalMargin = 0;

        foreach (var position in account.Positions.Where(p => p.Status == PositionStatus.Open))
        {
            var symbol = symbols[position.SymbolId];

            var margin = (position.Volume.Value * symbol.ContractSize * position.EntryPrice) / account.Leverage;

            totalMargin += margin;
        }

        return new Money(totalMargin, account.Balance.Currency);
    }
}
