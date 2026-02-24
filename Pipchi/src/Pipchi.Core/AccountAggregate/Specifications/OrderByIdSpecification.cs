using Ardalis.Specification;

namespace Pipchi.Core.AccountAggregate.Specifications;

public class OrderByIdSpecification : Specification<Account>
{
    public OrderByIdSpecification(Guid orderId)
    {
        Query.Where(account => account.Orders.Any(order => order.Id == orderId));
    }
}
