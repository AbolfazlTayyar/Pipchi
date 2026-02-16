using Pipchi.Core.Exceptions.Volume;
using Pipchi.SharedKernel;

namespace Pipchi.Core.ValueObjects;

public class Volume : ValueObject
{
    public Volume(decimal value)
    {
        if (value <= 0)
            throw new InvalidVolumeException("The volume specified is invalid. It must be greater than zero.");
        Value = value;
    }

    public decimal Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
