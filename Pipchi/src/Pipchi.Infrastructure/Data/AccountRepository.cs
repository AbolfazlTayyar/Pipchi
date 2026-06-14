using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.Interfaces;

namespace Pipchi.Infrastructure.Data;

public class AccountRepository : RepositoryBase<Account>, IAccountRepository
{
    public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Account>> ListWithKeysetPaginationAsync(
        DateTimeOffset? lastCreatedAt,
        Guid? lastId,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = DbContext.Set<Account>().AsQueryable();

        if (lastCreatedAt.HasValue)
        {
            query = query.Where(x =>
                x.CreatedAt > lastCreatedAt.Value ||
                (x.CreatedAt == lastCreatedAt.Value && x.Id.CompareTo(lastId!.Value) > 0));
        }

        return await query
            .OrderBy(x => x.CreatedAt)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public Task<List<Account>> SearchAsync(
        string currency,
        decimal? minBalance,
        decimal? maxBalance,
        int? leverage,
        DateTimeOffset? createdFrom,
        DateTimeOffset? createdTo,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = DbContext.Set<Account>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(currency))
            query = query.Where(a => a.Balance.Currency == currency.ToUpperInvariant());

        if (leverage.HasValue)
            query = query.Where(a => a.Leverage == leverage.Value);

        if (minBalance > 0)
            query = query.Where(a => a.Balance.Amount >= minBalance);

        if (maxBalance > 0)
            query = query.Where(a => a.Balance.Amount <= maxBalance);

        if (createdFrom.HasValue)
            query = query.Where(a => a.CreatedAt >= createdFrom.Value.UtcDateTime);

        if (createdTo.HasValue)
            query = query.Where(a => a.CreatedAt <= createdTo.Value.UtcDateTime);

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }
}
