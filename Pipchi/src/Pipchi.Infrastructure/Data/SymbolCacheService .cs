using Newtonsoft.Json;
using Pipchi.Core.Interfaces;
using Pipchi.Core.SyncedAggregates;
using Pipchi.SharedKernel.Interfaces;
using StackExchange.Redis;

namespace Pipchi.Infrastructure.Data;

public class SymbolCacheService : ISymbolCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IRepository<Symbol> _repository;
    private readonly IDatabase _db;
    private const string KEY_PREFIX = "symbol:";
    private const string ALL_KEY = "symbols:all";
    private readonly TimeSpan _expiration = TimeSpan.FromHours(24);

    public SymbolCacheService(IConnectionMultiplexer redis, IRepository<Symbol> repository)
    {
        _redis = redis;
        _repository = repository;
        _db = redis.GetDatabase();
    }

    public async Task<List<Symbol>> GetAllAsync(CancellationToken cancellationToken)
    {
        var cached = await _db.StringGetAsync(ALL_KEY);

        if (cached.HasValue)
            return JsonConvert.DeserializeObject<List<Symbol>>(cached.ToString()) ?? new();

        var symbols = await _repository.ListAsync(cancellationToken);
        await _db.StringSetAsync(ALL_KEY, JsonConvert.SerializeObject(symbols), _expiration);

        return symbols;
    }

    public async Task<Symbol?> GetByIdAsync(int id)
    {
        string key = $"{KEY_PREFIX}{id}";
        var cached = await _db.StringGetAsync(key);

        if (cached.HasValue)
            return JsonConvert.DeserializeObject<Symbol>(cached.ToString());

        var symbol = await _repository.GetByIdAsync(id);
        if (symbol != null)
            await _db.StringSetAsync(key, JsonConvert.SerializeObject(symbol), _expiration);

        return symbol;
    }

    public async Task InvalidateAsync(int id)
    {
        await _db.KeyDeleteAsync($"{KEY_PREFIX}{id}");
        await _db.KeyDeleteAsync(ALL_KEY);
    }

    public async Task InvalidateAllAsync()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        await foreach (var key in server.KeysAsync(pattern: $"{KEY_PREFIX}*"))
        {
            await _db.KeyDeleteAsync(key);
        }
        await _db.KeyDeleteAsync(ALL_KEY);
    }
}
