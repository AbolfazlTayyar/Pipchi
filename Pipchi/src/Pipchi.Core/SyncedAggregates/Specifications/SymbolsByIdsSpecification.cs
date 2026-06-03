using Ardalis.Specification;

namespace Pipchi.Core.SyncedAggregates.Specifications;

public class SymbolsByIdsSpecification : Specification<Symbol>
{
    public SymbolsByIdsSpecification(IReadOnlyCollection<int> symbolIds)
    {
        if (symbolIds == null || symbolIds.Count == 0)
        {
            Query.Where(_ => false);
            return;
        }

        Query.Where(s => symbolIds.Contains(s.Id));
    }
}