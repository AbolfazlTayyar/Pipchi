namespace Pipchi.Api.Models.Account;

public class CreateAccountRequest : BaseRequest
{
    public const string Route = "api/accounts";

    public string Currency { get; set; }
    public decimal Balance { get; set; }
}
