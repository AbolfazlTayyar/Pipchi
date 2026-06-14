using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Pipchi.Core.Interfaces;
using Pipchi.Core.SyncedAggregates;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Infrastructure.Data;

public class SymbolCacheService : ISymbolCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IRepository<Symbol> _repository;
    private const string KEY_PREFIX = "symbol:";
    private const string ALL_KEY = "symbols:all";
    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
    };

    public SymbolCacheService(IDistributedCache cache, IRepository<Symbol> repository)
    {
        _cache = cache;
        _repository = repository;
    }

    public async Task<List<Symbol>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cached = await _cache.GetStringAsync(ALL_KEY, cancellationToken);
        if (cached is not null)
            return JsonConvert.DeserializeObject<List<Symbol>>(cached) ?? new();

        var symbols = await _repository.ListAsync(cancellationToken);
        await _cache.SetStringAsync(ALL_KEY, JsonConvert.SerializeObject(symbols), _cacheOptions, cancellationToken);
        return symbols;
    }

    public async Task<Symbol?> GetByIdAsync(int id)
    {
        string key = $"{KEY_PREFIX}{id}";
        var cached = await _cache.GetStringAsync(key);
        if (cached is not null)
            return JsonConvert.DeserializeObject<Symbol>(cached);

        var symbol = await _repository.GetByIdAsync(id);
        if (symbol != null)
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(symbol), _cacheOptions);

        return symbol;
    }

    public async Task InvalidateAsync(int id)
    {
        await _cache.RemoveAsync($"{KEY_PREFIX}{id}");
        await _cache.RemoveAsync(ALL_KEY);
    }
}