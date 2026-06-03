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
}
