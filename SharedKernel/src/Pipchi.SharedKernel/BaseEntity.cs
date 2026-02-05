namespace Pipchi.SharedKernel;

public abstract class BaseEntity<TKey>
{
    public TKey Id { get; set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public List<BaseDomainEvent> Events { get; set; } = new();

    protected BaseEntity() =>  CreatedAt = DateTime.UtcNow;

    protected void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;
}
