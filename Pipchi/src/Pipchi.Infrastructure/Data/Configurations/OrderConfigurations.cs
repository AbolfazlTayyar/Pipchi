using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.SyncedAggregates;

namespace Pipchi.Infrastructure.Data.Configurations;

public class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(x => x.AccountId);

        builder.HasOne<Symbol>()
            .WithMany()
            .HasForeignKey(x => x.SymbolId);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.OwnsOne(x => x.Volume, volume =>
        {
            volume.Property(z => z.Value)
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.Property(x => x.EntryPrice)
            .HasPrecision(18, 5)
            .IsRequired();

        builder.Property(x => x.StopLoss)
            .HasPrecision(18, 5);

        builder.Property(x => x.TakeProfit)
            .HasPrecision(18, 5);

        builder.Ignore(x => x.Events);
    }
}
