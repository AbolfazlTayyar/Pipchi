using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pipchi.Core.AccountAggregate;

namespace Pipchi.Infrastructure.Data.Configurations;

public class AccountConfigurations : BaseEntityConfiguration<Account>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Account> builder)
    {
        builder.OwnsOne(x => x.Balance, balance =>
        {
            balance.Property(z => z.Amount)
                .HasPrecision(18, 2);

            balance.Property(z => z.Currency)
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasMany<Order>("_orders")
            .WithOne()
            .HasForeignKey(x => x.AccountId);

        builder.Ignore(x => x.Orders);
        builder.Ignore(x => x.Positions);
    }
}
