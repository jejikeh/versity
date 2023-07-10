using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeederConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4e274126-1d8a-4dfd-a025-806987095809",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "90eee6fd-4f43-4a4f-bc4f-cdf8dba15dd7", "AQAAAAIAAYagAAAAEH1Tti0lcnDSd2wCkPPwF8PAmWpnGy++NALRPt7IMPoKoveDLGds+X+FGjg6SD5Jjw==", "+000000000000", "89adadf9-6201-496e-918e-28d8e82dbd22" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4e274126-1d8a-4dfd-a025-806987095809",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "SecurityStamp" },
                values: new object[] { "3e28fcab-9803-4017-9b83-fb6d6f08f941", "AQAAAAIAAYagAAAAECZtpX3f5EdwYTFzJpAi86OG65M9CL32GtlvMmybz1546pkX3P2x7YEFURANs+zieg==", null, "4c83ee3a-99ef-4fa7-b6f4-bfa9e6c0d0f5" });
        }
    }
}
