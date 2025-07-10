using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CryptoPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class ComprehensiveAssetManagementSystemWithDataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_Cryptocurrencies_CryptocurrencyId",
                table: "Portfolios");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cryptocurrencies_CryptocurrencyId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_CryptocurrencyId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "AmountInDollars",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AveragePrice",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Portfolios");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Transactions",
                newName: "TransactionType");

            migrationBuilder.RenameColumn(
                name: "CryptocurrencyId",
                table: "Portfolios",
                newName: "UserId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerUnit",
                table: "Transactions",
                type: "numeric(18,8)",
                precision: 18,
                scale: 8,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,8)",
                oldPrecision: 18,
                oldScale: 8);

            migrationBuilder.AlterColumn<int>(
                name: "CryptocurrencyId",
                table: "Transactions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "AssetId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Transactions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PortfolioId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalTransactionValue",
                table: "Transactions",
                type: "numeric(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TransactionCurrency",
                table: "Transactions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BaseCurrency",
                table: "Portfolios",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Portfolios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Portfolios",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Portfolios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Portfolios",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AssetTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PortfolioId = table.Column<int>(type: "integer", nullable: false),
                    AssetTypeId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    AcquisitionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcquisitionCost = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    CurrentValue = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Location = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetTypes_AssetTypeId",
                        column: x => x.AssetTypeId,
                        principalTable: "AssetTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assets_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AssetTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Digital or virtual currencies secured by cryptography", "Cryptocurrency" },
                    { 2, "Shares of ownership in a corporation", "Stock" },
                    { 3, "Property consisting of land and buildings", "Real Estate" },
                    { 4, "Cars, motorcycles, boats, and other vehicles", "Vehicle" },
                    { 5, "Raw materials or primary agricultural products", "Commodity" },
                    { 6, "Items collected for their rarity, beauty, or historical significance", "Collectibles" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "LastUpdated", "PasswordHash", "Username" },
                values: new object[] { 1, new DateTime(2025, 7, 9, 2, 25, 6, 716, DateTimeKind.Utc).AddTicks(60), "default@cryptoportfolio.com", new DateTime(2025, 7, 9, 2, 25, 6, 716, DateTimeKind.Utc).AddTicks(60), "default_hash_placeholder", "defaultuser" });

            // Update existing portfolios to use the default user and set default values
            migrationBuilder.Sql(@"
                UPDATE ""Portfolios"" 
                SET 
                    ""UserId"" = 1,
                    ""Name"" = 'Legacy Portfolio',
                    ""Description"" = 'Migrated from old crypto-only system',
                    ""BaseCurrency"" = 'USD',
                    ""CreatedAt"" = NOW(),
                    ""IsActive"" = true
                WHERE ""UserId"" IS NULL OR ""UserId"" = 0;
            ");

            // Create a default asset for existing transactions
            migrationBuilder.Sql(@"
                INSERT INTO ""Assets"" (""PortfolioId"", ""AssetTypeId"", ""Name"", ""Symbol"", ""Description"", ""Quantity"", ""AcquisitionDate"", ""AcquisitionCost"", ""Currency"", ""LastUpdated"", ""CreatedAt"", ""IsActive"")
                VALUES (1, 1, 'Legacy Cryptocurrency Holdings', 'LEGACY', 'Migrated cryptocurrency holdings from old system', 0, NOW(), 0, 'USD', NOW(), NOW(), true);
            ");

            // Update existing transactions to set default values
            migrationBuilder.Sql(@"
                UPDATE ""Transactions"" 
                SET 
                    ""TotalTransactionValue"" = ""Amount"" * COALESCE(""PricePerUnit"", 0),
                    ""TransactionCurrency"" = 'USD',
                    ""CreatedAt"" = ""TransactionDate"",
                    ""AssetId"" = 1,
                    ""PortfolioId"" = 1
                WHERE ""TotalTransactionValue"" = 0;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AssetId",
                table: "Transactions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PortfolioId",
                table: "Transactions",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_UserId",
                table: "Portfolios",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetTypeId",
                table: "Assets",
                column: "AssetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_PortfolioId",
                table: "Assets",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_Name",
                table: "AssetTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_Users_UserId",
                table: "Portfolios",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Assets_AssetId",
                table: "Transactions",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cryptocurrencies_CryptocurrencyId",
                table: "Transactions",
                column: "CryptocurrencyId",
                principalTable: "Cryptocurrencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Portfolios_PortfolioId",
                table: "Transactions",
                column: "PortfolioId",
                principalTable: "Portfolios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Portfolios_Users_UserId",
                table: "Portfolios");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Assets_AssetId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cryptocurrencies_CryptocurrencyId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Portfolios_PortfolioId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AssetTypes");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AssetId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PortfolioId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Portfolios_UserId",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PortfolioId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TotalTransactionValue",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionCurrency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BaseCurrency",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Portfolios");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Portfolios",
                newName: "CryptocurrencyId");

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerUnit",
                table: "Transactions",
                type: "numeric(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,8)",
                oldPrecision: 18,
                oldScale: 8,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CryptocurrencyId",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountInDollars",
                table: "Transactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AveragePrice",
                table: "Portfolios",
                type: "numeric(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Portfolios",
                type: "numeric(18,8)",
                precision: 18,
                scale: 8,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_CryptocurrencyId",
                table: "Portfolios",
                column: "CryptocurrencyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolios_Cryptocurrencies_CryptocurrencyId",
                table: "Portfolios",
                column: "CryptocurrencyId",
                principalTable: "Cryptocurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cryptocurrencies_CryptocurrencyId",
                table: "Transactions",
                column: "CryptocurrencyId",
                principalTable: "Cryptocurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
