namespace Pipchi.Api.Models.Account;

public class ListAccountRequest : BaseRequest
{
    public const string Route = "api/accounts";
    public int? PageSize { get; set; } = 30;
    public Guid? LastId { get; set; }
    public DateTime? LastCreatedAt { get; set; }
}
