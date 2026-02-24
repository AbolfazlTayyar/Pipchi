using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pipchi.Core.AccountAggregate;
using Pipchi.Core.SyncedAggregates;

namespace Pipchi.Infrastructure.Data.Configurations;

public class OrderConfigurations : BaseEntityConfiguration<Order>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.Type)
            .HasMaxLength(ColumnConstants.DEFAULT_TRADE_TYPE_ENUM_LENGTH)
            .IsRequired();

        builder.OwnsOne(x => x.Volume, volume =>
        {
            volume.Property(z => z.Value)
                .HasColumnName("Volume")
                .HasPrecision(18, 2);
        });

        builder.Property(x => x.EntryPrice)
            .HasPrecision(18, 5);

        builder.Property(x => x.StopLoss)
            .HasPrecision(18, 5);

        builder.Property(x => x.TakeProfit)
            .HasPrecision(18, 5);
    }
}
