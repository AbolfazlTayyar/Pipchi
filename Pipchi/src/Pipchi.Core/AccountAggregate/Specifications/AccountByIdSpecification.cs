using Ardalis.Specification;

namespace Pipchi.Core.AccountAggregate.Specifications;

public class AccountByIdSpecification : Specification<Account>
{
	public AccountByIdSpecification(Guid id)
	{
		Query.Where(x => x.Id == id);
    }
}
