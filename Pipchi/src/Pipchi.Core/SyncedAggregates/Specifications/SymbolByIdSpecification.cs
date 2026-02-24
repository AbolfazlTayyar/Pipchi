using Ardalis.Specification;

namespace Pipchi.Core.SyncedAggregates.Specifications;

public class SymbolByIdSpecification : Specification<Symbol>
{
    public SymbolByIdSpecification(int id)
    {
        Query.Where(s => s.Id == id);
    }
}
