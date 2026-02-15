using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pipchi.Core.SyncedAggregates;

namespace Pipchi.Infrastructure.Data.Configurations;

public class SymbolConfigurations : BaseEntityConfiguration<Symbol, int>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Symbol> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ColumnConstants.DEFAULT_NAME_LENGTH);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.MinPrice)
            .HasPrecision(18, 5);

        builder.Property(x => x.MaxPrice)
            .HasPrecision(18, 5);

        builder.Property(x => x.MinVolume)
            .HasPrecision(18, 2);

        builder.Property(x => x.MaxVolume)
            .HasPrecision(18, 2);
    }
}