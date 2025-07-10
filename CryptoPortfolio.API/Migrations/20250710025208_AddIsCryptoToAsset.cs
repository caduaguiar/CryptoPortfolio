using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCryptoToAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCrypto",
                table: "Assets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastUpdated" },
                values: new object[] { new DateTime(2025, 7, 10, 2, 52, 8, 118, DateTimeKind.Utc).AddTicks(1820), new DateTime(2025, 7, 10, 2, 52, 8, 118, DateTimeKind.Utc).AddTicks(1820) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCrypto",
                table: "Assets");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastUpdated" },
                values: new object[] { new DateTime(2025, 7, 9, 2, 25, 6, 716, DateTimeKind.Utc).AddTicks(60), new DateTime(2025, 7, 9, 2, 25, 6, 716, DateTimeKind.Utc).AddTicks(60) });
        }
    }
}
