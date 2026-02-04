namespace Pipchi.Core.Exceptions;

public class IncompatibleCurrencyException : Exception
{
    public IncompatibleCurrencyException() : base("The currencies are incompatible for this operation.")
    {
        
    }
}
