using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pipchi.Infrastructure.Outbox;

namespace Pipchi.Infrastructure.Data.Configurations;

internal sealed class OutboxMessageConfigurations : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.Ignore(x => x.Events);

        builder.Ignore(x => x.CreatedAt);
        
        builder.Ignore(x => x.UpdatedAt);
    }
}
