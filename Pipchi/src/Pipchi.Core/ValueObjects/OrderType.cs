using Pipchi.SharedKernel;

namespace Pipchi.Core.ValueObjects;

public class OrderType : ValueObject
{
    private OrderType(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public static readonly OrderType Buy = new("Buy");
    public static readonly OrderType Sell = new("Sell");

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
