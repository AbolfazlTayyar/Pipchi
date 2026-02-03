namespace Pipchi.SharedKernel;

public abstract class BaseEntity<TKey>
{
    public TKey Id { get; set; }
    public List<BaseDomainEvent> Events { get; set; } = new();
}
