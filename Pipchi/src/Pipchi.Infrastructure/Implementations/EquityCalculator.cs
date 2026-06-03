using Pipchi.Core.AccountAggregate;
using Pipchi.Core.Enums;
using Pipchi.Core.Interfaces;
using Pipchi.Core.ValueObjects;

namespace Pipchi.Infrastructure.Implementations;

public class EquityCalculator : IEquityCalculator
{
    public Money Calculate(Account account)
    {
        var floatingPnL = account.Positions
            .Where(p => p.Status == PositionStatus.Open)
            .Sum(p => p.Profit ?? 0m);

        return new Money(account.Balance.Amount + floatingPnL, account.Balance.Currency);
    }
}
