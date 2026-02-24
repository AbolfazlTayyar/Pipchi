namespace Pipchi.Core.Exceptions.Account;

internal class MarketCloseException : Exception
{
    public MarketCloseException(string message) : base(message)
    {
    }
}
