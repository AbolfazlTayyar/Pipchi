using Pipchi.Core.AccountAggregate;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Core.Interfaces;

public interface IUsedMarginCalculator
{
    Money Calculate(Account account, IReadOnlyDictionary<int, Symbol> symbols);
}
