using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pipchi.Core.AccountAggregate;

namespace Pipchi.Infrastructure.Data.Configurations;

public class AccountConfigurations : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.OwnsOne(x => x.Balance, balance =>
        {
            balance.Property(z => z.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            balance.Property(z => z.Currency)
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasMany<Order>("_orders")
            .WithOne()
            .HasForeignKey(x => x.AccountId);

        builder.Ignore(x => x.Orders);
        builder.Ignore(x => x.Positions);
        builder.Ignore(x => x.Events);
    }
}
