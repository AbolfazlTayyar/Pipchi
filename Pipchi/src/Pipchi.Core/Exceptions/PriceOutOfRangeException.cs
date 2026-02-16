namespace Pipchi.Core.Exceptions;

public class PriceOutOfRangeException : Exception
{
    public PriceOutOfRangeException(string message) : base(message) { }
}
