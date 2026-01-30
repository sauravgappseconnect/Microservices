using CommandService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommandService.Data.DataConfiguration
{
    public class CommandConfiguration : IEntityTypeConfiguration<Command>
    {
        public void Configure(EntityTypeBuilder<Command> builder)
        {
            builder.ToTable(nameof(Command));

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWID()");

            builder.Property(e => e.HowTo)
            .HasMaxLength(3000)
            .IsRequired();

            builder.Property(e => e.CommandLine)
            .HasMaxLength(3000)
            .IsRequired();

            builder.HasOne(c=>c.Platform)
                .WithMany(p=>p.Commands)
                .HasForeignKey(c=>c.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
