using Pipchi.Core.SyncedAggregates;

namespace Pipchi.Core.Interfaces;

public interface ISymbolCacheService
{
    Task<Symbol?> GetByIdAsync(int id);
    Task<List<Symbol>> GetAllAsync(CancellationToken cancellationToken);
    Task InvalidateAsync(int id);
}
