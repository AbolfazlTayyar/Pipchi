using Ardalis.Specification;

namespace Pipchi.Core.SyncedAggregates.Specifications;

public class SymbolByNameSpecification : Specification<Symbol>
{
    public SymbolByNameSpecification(string name)
    {
        Query.Where(x => x.Name == name);
    }
}
