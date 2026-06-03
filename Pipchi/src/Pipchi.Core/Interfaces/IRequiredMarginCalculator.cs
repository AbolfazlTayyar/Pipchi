using Pipchi.Core.AccountAggregate;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Core.Interfaces;

public interface IRequiredMarginCalculator
{
    Money Calculate(Account account, Symbol symbol, decimal volume, decimal price);
}
