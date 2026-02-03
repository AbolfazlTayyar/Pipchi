using MediatR;

namespace Pipchi.SharedKernel;

public abstract class BaseDomainEvent : INotification
{
    public DateTimeOffset DateOccured { get; protected set; } = DateTimeOffset.UtcNow;
}