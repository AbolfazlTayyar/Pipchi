namespace Pipchi.Core.Exceptions;

public class InvalidPriceFormatException : Exception
{
    public InvalidPriceFormatException(string message) : base(message)
    {
    }
}
