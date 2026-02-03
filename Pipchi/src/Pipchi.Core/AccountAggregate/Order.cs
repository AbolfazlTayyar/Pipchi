using Pipchi.Core.ValueObjects;
using Pipchi.SharedKernel;

namespace Pipchi.Core.AccountAggregate;

public class Order : BaseEntity<Guid>
{
    public OrderType Type { get; private set; }
    public int AccountId { get; private set; }
}
