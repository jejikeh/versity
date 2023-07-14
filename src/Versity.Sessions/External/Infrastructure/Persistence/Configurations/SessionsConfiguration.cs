using Domain.Models;
using Domain.Models.SessionLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SessionsConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder
            .HasOne(e => e.Logs)
            .WithOne(e => e.Session)
            .HasForeignKey<SessionLogs>(e => e.SessionId)
            .IsRequired();
    }
}