using Ardalis.Specification.EntityFrameworkCore;
using Pipchi.SharedKernel.Interfaces;

namespace Pipchi.Infrastructure.Data;

public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(ApplicationDbContext dbContext) 
        : base(dbContext)
    {
    }
}
