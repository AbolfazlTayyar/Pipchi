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
}
