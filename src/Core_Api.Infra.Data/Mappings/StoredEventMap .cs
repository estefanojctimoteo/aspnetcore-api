using Core_Api.Domain.Core.Events;
using Core_Api.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core_Api.Infra.Data.Mappings
{
    public class StoredEventMap : EntityTypeConfiguration<StoredEvent>
    {
        public override void Map(EntityTypeBuilder<StoredEvent> builder)
        {
            builder.Property(c => c.Timestamp)
                .HasColumnName("CreationDate");

            builder.Property(c => c.MessageType)
                .HasColumnName("Action")
                .HasColumnType("varchar(100)");

        }
    }
}