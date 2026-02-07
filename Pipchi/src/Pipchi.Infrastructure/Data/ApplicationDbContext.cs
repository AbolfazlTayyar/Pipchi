using Microsoft.EntityFrameworkCore;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.SyncedAggregates;
using Pipchi.Infrastructure.Data.Extensions;
using System.Reflection;

namespace Pipchi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Symbol> Symbols { get; set; }

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
}
