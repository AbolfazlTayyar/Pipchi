using Pipchi.Core.Interfaces;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Core.SyncedAggregates.Specifications;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Infrastructure.Services;

public class SymbolUniquenessChecker : ISymbolUniquenessChecker
{
    private readonly IReadRepository<Symbol> _repository;

    public SymbolUniquenessChecker(IReadRepository<Symbol> repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken)
    {
        var spec = new SymbolByNameSpecification(name);
        var count = await _repository.CountAsync(spec, cancellationToken);
        return count == 0;
    }
}
