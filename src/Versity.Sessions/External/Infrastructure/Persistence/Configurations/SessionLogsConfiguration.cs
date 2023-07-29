using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SessionLogsConfiguration : IEntityTypeConfiguration<SessionLogs>
{
    public void Configure(EntityTypeBuilder<SessionLogs> builder)
    {
        builder
            .HasMany(e => e.Logs)
            .WithOne(e => e.SessionLogs)
            .HasForeignKey(e => e.SessionLogsId)
            .IsRequired();
    }
}