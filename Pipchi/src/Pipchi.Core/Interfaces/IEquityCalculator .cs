using Pipchi.Core.AccountAggregate;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Core.Interfaces;

public interface IEquityCalculator
{
    Money Calculate(Account account);
}
