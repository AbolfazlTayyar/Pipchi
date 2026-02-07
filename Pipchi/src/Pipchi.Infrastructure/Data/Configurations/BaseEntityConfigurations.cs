using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pipchi.SharedKernel;

namespace Pipchi.Infrastructure.Data.Configurations;

public abstract class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Ignore(x => x.Events);

        ConfigureEntity(builder);
    }

    // This method can be overridden in derived classes to provide additional configuration specific to the entity.
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
