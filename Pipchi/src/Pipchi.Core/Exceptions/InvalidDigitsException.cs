namespace Pipchi.Core.Exceptions;

public class InvalidDigitsException : Exception
{
    public InvalidDigitsException() : base("Digits cannot exceed 5") 
    {
        
    }
}
