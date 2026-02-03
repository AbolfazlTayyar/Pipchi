using Pipchi.SharedKernel;

namespace Pipchi.Core.ValueObjects;

public class Money : ValueObject
{
    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public string Currency { get; init; }
    public decimal Amount { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Currency;
        yield return Amount;
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        return new Money(a.Amount - b.Amount, a.Currency);
    }
}