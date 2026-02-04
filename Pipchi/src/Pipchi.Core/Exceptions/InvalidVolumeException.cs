namespace Pipchi.Core.Exceptions;

public class InvalidVolumeException : Exception
{
    public InvalidVolumeException() : base("The volume specified is invalid. It must be greater than zero.")
    {
    }
}
