namespace Pipchi.Api.Models.Account;

public class AccountDto
{
    public Guid Id { get; set; }
    public string Currency { get; set; }
    public decimal Balance { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int Leverage { get; set; }
}
