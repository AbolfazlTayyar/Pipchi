namespace Pipchi.Api.Models.Account;

public class CreateAccountResponse : BaseResponse
{
    public CreateAccountResponse(Guid correlationId) : base(correlationId)
    {

    }

    public AccountDto AccountDto { get; set; } = new();
}
