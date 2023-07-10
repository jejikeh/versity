using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeederFixEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4e274126-1d8a-4dfd-a025-806987095809",
                columns: new[] { "ConcurrencyStamp", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "940b9862-24d0-4706-b35f-4d71f489a3f5", "VERSITY.IDENTITY.DEV@GMAIL.COM", "AQAAAAIAAYagAAAAEHJAUFLF091zTtWBRh1eqdVvnw4Iuxl8qxz+A507cK+anUfVKN+qdRTimf8YMLX+fw==", "8dddf7d4-5da6-4c9e-be01-d56574d21315" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4e274126-1d8a-4dfd-a025-806987095809",
                columns: new[] { "ConcurrencyStamp", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "90eee6fd-4f43-4a4f-bc4f-cdf8dba15dd7", null, "AQAAAAIAAYagAAAAEH1Tti0lcnDSd2wCkPPwF8PAmWpnGy++NALRPt7IMPoKoveDLGds+X+FGjg6SD5Jjw==", "89adadf9-6201-496e-918e-28d8e82dbd22" });
        }
    }
}
