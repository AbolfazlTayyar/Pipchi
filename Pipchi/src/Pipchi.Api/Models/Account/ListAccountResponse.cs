namespace Pipchi.Api.Models.Account;

public class ListAccountResponse : BaseResponse
{
    public ListAccountResponse(Guid correlationId) : base(correlationId)
    {

    }

    public List<AccountDto> Accounts { get; set; } = new();
    public DateTimeOffset NextCursor { get; set; }
    public bool HasMore { get; set; }
}
