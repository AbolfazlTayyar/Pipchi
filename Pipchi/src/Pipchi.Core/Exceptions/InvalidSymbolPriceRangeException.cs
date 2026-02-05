namespace Pipchi.Core.Exceptions;

public class InvalidSymbolPriceRangeException : Exception
{
    public InvalidSymbolPriceRangeException() : base("Invalid price range for symbol. MinPrice must be less than MaxPrice.")
    {

    }
}
