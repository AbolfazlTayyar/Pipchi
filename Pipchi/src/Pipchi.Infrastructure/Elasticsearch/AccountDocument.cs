namespace Pipchi.Infrastructure.Elasticsearch;

public class AccountDocument
{
    public Guid Id { get; set; }

    public string BalanceCurrency { get; set; }

    public decimal BalanceAmount { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public int Leverage { get; set; }
}