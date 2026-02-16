using Ardalis.GuardClauses;
using Pipchi.Core.Exceptions;
using Pipchi.Core.Exceptions.Volume;
using Pipchi.SharedKernel;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Core.SyncedAggregates;

public class Symbol : BaseEntity<int>, IAggregateRoot
{
    public Symbol(string name,
        int digits,
        decimal minPrice,
        decimal maxPrice,
        decimal minVolume,
        decimal maxVolume)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Digits = Guard.Against.NegativeOrZero(digits, nameof(digits));
        MinPrice = Guard.Against.NegativeOrZero(minPrice, nameof(minPrice));
        MaxPrice = Guard.Against.NegativeOrZero(maxPrice, nameof(maxPrice));
        MinVolume = Guard.Against.NegativeOrZero(minVolume, nameof(minVolume));
        MaxVolume = Guard.Against.NegativeOrZero(maxVolume, nameof(maxVolume));
    }

    public string Name { get; private set; }
    public int Digits { get; private set; }  // 5 for EURUSD, 3 for USDJPY
    public decimal MinPrice { get; private set; }
    public decimal MaxPrice { get; private set; }
    public decimal MinVolume { get; private set; }
    public decimal MaxVolume { get; private set; }

    private decimal NormalizePrice(decimal price) => Math.Round(price, Digits);

    public void ValidatePrice(decimal price)
    {
        if (price < MinPrice || price > MaxPrice)
            throw new PriceOutOfRangeException($"Price must be between {MinPrice} and {MaxPrice}");

        if (price != NormalizePrice(price))
            throw new InvalidPriceFormatException($"Price must have {Digits} decimal places");
    }

    public void UpdateName(string name)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));

        MarkAsUpdated();
    }

    public void UpdatePriceRange(decimal minPrice, decimal maxPrice)
    {
        Guard.Against.NegativeOrZero(minPrice, nameof(minPrice));
        Guard.Against.NegativeOrZero(maxPrice, nameof(maxPrice));

        if (minPrice >= maxPrice)
            throw new InvalidSymbolPriceRangeException();

        MinPrice = minPrice;
        MaxPrice = maxPrice;

        MarkAsUpdated();
    }

    public void UpdateVolumeRange(decimal minVolume, decimal maxVolume)
    {
        Guard.Against.NegativeOrZero(minVolume, nameof(minVolume));
        Guard.Against.NegativeOrZero(maxVolume, nameof(maxVolume));

        if (minVolume >= maxVolume)
            throw new InvalidVolumeRangeException("The specified volume range is invalid.");

        MinVolume = minVolume;
        MaxVolume = maxVolume;

        MarkAsUpdated();
    }

    public void UpdateDigits(int digits)
    {
        Guard.Against.NegativeOrZero(digits, nameof(digits));

        if (digits > 5)
            throw new InvalidDigitsException();

        Digits = digits;

        MarkAsUpdated();
    }

    public void ValidateVolume(decimal volume)
    {
        if (volume < MinVolume || volume > MaxVolume)
            throw new InvalidVolumeException($"Volume must be between {MinVolume} and {MaxVolume}");
    }

    public override string ToString() => Name.ToString();
}
