using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.SyncedAggregates;

namespace Pipchi.Infrastructure.Data.Configurations;

public class PositionConfigurations : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(x => x.OrderId);

        builder.HasOne<Symbol>()
            .WithMany()
            .HasForeignKey(x => x.SymbolId);

        builder.OwnsOne(x => x.Volume, volume =>
        {
            volume.Property(z => z.Value)
                .HasPrecision(18, 2)
                .IsRequired();
        });

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

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
