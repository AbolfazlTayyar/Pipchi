using Ardalis.GuardClauses;
using Pipchi.Core.Exceptions;
using Pipchi.Core.Exceptions.Account;
using Pipchi.Core.Exceptions.Symbol;
using Pipchi.Core.Exceptions.Volume;
using Pipchi.SharedKernel;
using Pipchi.SharedKernel.Extensions;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Core.SyncedAggregates;

public class Symbol : BaseEntity<int>, IAggregateRoot
{
    public Symbol(string name,
        int digits,
        decimal minPrice,
        decimal maxPrice,
        decimal minVolume,
        decimal maxVolume,
        decimal volumeStep,
        int contractSize,
        TimeOnly? marketOpenTime = null,
        TimeOnly? marketCloseTime = null)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Digits = Guard.Against.NegativeOrZero(digits, nameof(digits));
        MinPrice = Guard.Against.NegativeOrZero(minPrice, nameof(minPrice));
        MaxPrice = Guard.Against.NegativeOrZero(maxPrice, nameof(maxPrice));
        MinVolume = Guard.Against.NegativeOrZero(minVolume, nameof(minVolume));
        MaxVolume = Guard.Against.NegativeOrZero(maxVolume, nameof(maxVolume));
        VolumeStep = Guard.Against.NegativeOrZero(volumeStep, nameof(volumeStep));
        ContractSize = Guard.Against.NegativeOrZero(contractSize, nameof(contractSize));
        MarketOpenTime = marketOpenTime ?? TimeOnly.MinValue;
        MarketCloseTime = marketCloseTime ?? TimeOnly.MaxValue;
    }

    private Symbol() { }

    public string Name { get; private set; }
    public int Digits { get; private set; }  // 5 for EURUSD, 3 for USDJPY
    public decimal MinPrice { get; private set; }
    public decimal MaxPrice { get; private set; }
    public decimal MinVolume { get; private set; }
    public decimal MaxVolume { get; private set; }
    public decimal VolumeStep { get; private set; }
    public int ContractSize { get; private set; }
    public TimeOnly MarketOpenTime { get; private set; }
    public TimeOnly MarketCloseTime { get; private set; }

    private decimal NormalizePrice(decimal price) => Math.Round(price, Digits);

    private decimal NormalizeVolume(decimal volume)
    {
        var steps = Math.Round(volume / VolumeStep);
        return steps * VolumeStep;
    }

    public void ValidatePrice(decimal price, string parameterName)
    {
        if (price < MinPrice || price > MaxPrice)
            throw new PriceOutOfRangeException($"{parameterName} Price must be between {MinPrice} and {MaxPrice}");

        if (price != NormalizePrice(price))
            throw new InvalidPriceFormatException($"{parameterName} Price must have {Digits} decimal places");
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
            throw new InvalidVolumeException($"Volume must be between {MinVolume.Trimmed()} and {MaxVolume.Trimmed()}");

        if (volume != NormalizeVolume(volume))
            throw new InvalidVolumeException($"Volume must have {VolumeStep.Trimmed()} volume step");
    }

    public void EnsureMarketOpen(DateTimeOffset checkTime)
    {
        var currentDay = checkTime.DayOfWeek;
        var currentTime = TimeOnly.FromTimeSpan(checkTime.TimeOfDay);

        //if (currentDay == DayOfWeek.Saturday || currentDay == DayOfWeek.Sunday)
        //    throw new MarketCloseException($"Market for {Name} is closed on weekends");

        bool isWithinTradingHours;
        if (MarketOpenTime <= MarketCloseTime)
            isWithinTradingHours = currentTime >= MarketOpenTime && currentTime <= MarketCloseTime;
        else
            isWithinTradingHours = currentTime >= MarketOpenTime || currentTime <= MarketCloseTime;

        if (!isWithinTradingHours)
            throw new MarketCloseException($"Market for {Name} is closed. Trading hours: {MarketOpenTime} - {MarketCloseTime}");
    }

    public override string ToString() => Name.ToString();
}
