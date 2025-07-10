using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyConversionSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AcquisitionCostUSD",
                table: "Assets",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentValueUSD",
                table: "Assets",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeRateLastUpdated",
                table: "Assets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRateToUSD",
                table: "Assets",
                type: "numeric",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastUpdated" },
                values: new object[] { new DateTime(2025, 7, 10, 3, 56, 38, 847, DateTimeKind.Utc).AddTicks(9030), new DateTime(2025, 7, 10, 3, 56, 38, 847, DateTimeKind.Utc).AddTicks(9030) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcquisitionCostUSD",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CurrentValueUSD",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ExchangeRateLastUpdated",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "ExchangeRateToUSD",
                table: "Assets");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastUpdated" },
                values: new object[] { new DateTime(2025, 7, 10, 2, 52, 8, 118, DateTimeKind.Utc).AddTicks(1820), new DateTime(2025, 7, 10, 2, 52, 8, 118, DateTimeKind.Utc).AddTicks(1820) });
        }
    }
}
