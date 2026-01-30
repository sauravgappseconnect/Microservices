

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformService.Models;

namespace PlatformService.Data.DataConfiguration
{
    public class PlatformConfiguration : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            builder.ToTable(nameof(Platform));

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()");

            builder.Property(e => e.Name)
            .HasMaxLength(500)
            .IsRequired();

            builder.Property(e => e.Publisher)
            .HasMaxLength(500)
            .IsRequired();

            builder.Property(e => e.Cost)
            .IsRequired();

        }
    }
}