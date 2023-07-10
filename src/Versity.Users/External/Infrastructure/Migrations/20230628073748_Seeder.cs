using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Seeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a337651d-2193-4c2d-bfe3-4cccc2ac82fa", null, "Member", null },
                    { "e56d08b9-8788-4c58-958a-1a7bcb585fc2", null, "Admin", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4e274126-1d8a-4dfd-a025-806987095809", 0, "3e28fcab-9803-4017-9b83-fb6d6f08f941", "versity.identity.dev@gmail.com", true, "Versity", "Admin", false, null, null, null, "AQAAAAIAAYagAAAAECZtpX3f5EdwYTFzJpAi86OG65M9CL32GtlvMmybz1546pkX3P2x7YEFURANs+zieg==", null, false, "4c83ee3a-99ef-4fa7-b6f4-bfa9e6c0d0f5", false, "Versity Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "e56d08b9-8788-4c58-958a-1a7bcb585fc2", "4e274126-1d8a-4dfd-a025-806987095809" });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_Token",
                table: "RefreshTokens",
                columns: new[] { "UserId", "Token" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserId_Token",
                table: "RefreshTokens");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a337651d-2193-4c2d-bfe3-4cccc2ac82fa");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "e56d08b9-8788-4c58-958a-1a7bcb585fc2", "4e274126-1d8a-4dfd-a025-806987095809" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e56d08b9-8788-4c58-958a-1a7bcb585fc2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4e274126-1d8a-4dfd-a025-806987095809");
        }
    }
}
