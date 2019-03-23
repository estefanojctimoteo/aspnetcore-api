using Core_Api.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core_Api.Domain.AppUsers;

namespace Core_Api.Infra.Data.Mappings
{
    public class AppUserMapping : EntityTypeConfiguration<AppUser>
    {
        public override void Map(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(e => e.Email)
               .HasColumnType("varchar(100)")
               .IsRequired();

            builder.Ignore(e => e.ValidationResult);

            builder.Ignore(e => e.CascadeMode);

            builder.Ignore(e => e.Name);

            builder.ToTable("AppUser");
        }
    }
}