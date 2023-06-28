using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class RolesSeederConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole("Admin") { Id = "e56d08b9-8788-4c58-958a-1a7bcb585fc2", NormalizedName = "ADMIN" },
            new IdentityRole("Member") { Id = "a337651d-2193-4c2d-bfe3-4cccc2ac82fa", NormalizedName = "MEMBER" });
    }
}