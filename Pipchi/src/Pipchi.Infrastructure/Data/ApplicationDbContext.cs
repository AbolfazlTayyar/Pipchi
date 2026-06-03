using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Infrastructure.Data.Extensions;
using Pipchi.Infrastructure.Outbox;
using Pipchi.SharedKernel;
using Pipchi.SharedKernel.Exceptions;
using System.Reflection;

namespace Pipchi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private static readonly new JsonSerializerSettings jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Symbol> Symbols { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.AddRestrictDeleteBehaviorConvention();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Configure all enums to be stored as strings in the database for better readability and maintainability
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages();  

            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occured.", ex);
        }
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var now = DateTime.UtcNow;

        var outboxMessages = ChangeTracker
            .Entries<BaseEntity<Guid>>()
            .Select(e => e.Entity)
            .SelectMany(e => e.Events)
            .Select(e => new OutboxMessage(
                Guid.NewGuid(),
                now,
                e.GetType().Name,
                JsonConvert.SerializeObject(e, jsonSerializerSettings)))
            .ToList();

        AddRange(outboxMessages);
    }
}
