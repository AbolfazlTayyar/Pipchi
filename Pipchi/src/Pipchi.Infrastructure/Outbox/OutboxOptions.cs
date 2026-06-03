namespace Pipchi.Infrastructure.Outbox;

internal sealed class OutboxOptions
{
    public int IntervalInSeconds { get; set; }
    public int BatchSize { get; set; }
}
