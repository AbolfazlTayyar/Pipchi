using Pipchi.Core.AccountAggregate;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Core.Interfaces;

public interface IMarginCalculator
{
    Money Calculate(Account account);
}
