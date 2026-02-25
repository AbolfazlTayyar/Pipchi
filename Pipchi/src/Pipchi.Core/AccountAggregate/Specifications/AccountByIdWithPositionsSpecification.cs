using Ardalis.Specification;

namespace Pipchi.Core.AccountAggregate.Specifications;

public class AccountByIdWithPositionsSpecification : Specification<Account>
{
    public AccountByIdWithPositionsSpecification(Guid id)
    {
        Query.Include(a => a.Positions)
            .Where(a => a.Id == id);
    }
}
