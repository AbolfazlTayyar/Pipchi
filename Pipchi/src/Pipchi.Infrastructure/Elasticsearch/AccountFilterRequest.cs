namespace Pipchi.Api.Models.Account;

public class AccountFilterRequest
{
    public string? Currency { get; set; }
    public decimal? MinBalance { get; set; }
    public decimal? MaxBalance { get; set; }
    public int? Leverage { get; set; }
    public DateTimeOffset? CreatedFrom { get; set; }
    public DateTimeOffset? CreatedTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
