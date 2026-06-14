using Pipchi.Core.AccountAggregate;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Core.Interfaces;

public interface IAccountRepository : IRepository<Account>, IAggregateRoot
{
    Task<List<Account>> ListWithKeysetPaginationAsync(
        DateTimeOffset? lastCreatedAt,
        Guid? lastId,
        int pageSize,
        CancellationToken ct = default);

    Task<List<Account>> SearchAsync(
        string currency,
        decimal? minBalance,
        decimal? maxBalance,
        int? leverage,
        DateTimeOffset? createdFrom,
        DateTimeOffset? createdTo,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
}
