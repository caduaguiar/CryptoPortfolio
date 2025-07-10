using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoPortfolio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CryptoPortfolio.Data
{
    public class CryptoDbContext : DbContext
    {
        public CryptoDbContext(DbContextOptions<CryptoDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
        // Keep for backward compatibility during migration
        public DbSet<Cryptocurrency> Cryptocurrencies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Portfolio configuration
            modelBuilder.Entity<Portfolio>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.User)
                      .WithMany(e => e.Portfolios)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // AssetType configuration
            modelBuilder.Entity<AssetType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Asset configuration
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).HasPrecision(18, 8);
                entity.Property(e => e.AcquisitionCost).HasPrecision(18, 8);
                entity.Property(e => e.CurrentValue).HasPrecision(18, 8);

                entity.HasOne(e => e.Portfolio)
                      .WithMany(e => e.Assets)
                      .HasForeignKey(e => e.PortfolioId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AssetType)
                      .WithMany(e => e.Assets)
                      .HasForeignKey(e => e.AssetTypeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Transaction configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 8);
                entity.Property(e => e.PricePerUnit).HasPrecision(18, 8);
                entity.Property(e => e.TotalTransactionValue).HasPrecision(18, 8);

                entity.HasOne(e => e.Asset)
                      .WithMany(e => e.Transactions)
                      .HasForeignKey(e => e.AssetId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Portfolio)
                      .WithMany(e => e.Transactions)
                      .HasForeignKey(e => e.PortfolioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Cryptocurrency configuration (keep for backward compatibility)
            modelBuilder.Entity<Cryptocurrency>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Symbol).IsUnique();
                entity.Property(e => e.CurrentPrice).HasPrecision(18, 8);
            });

            // Seed initial asset types
            modelBuilder.Entity<AssetType>().HasData(
                new AssetType { Id = 1, Name = "Cryptocurrency", Description = "Digital or virtual currencies secured by cryptography" },
                new AssetType { Id = 2, Name = "Stock", Description = "Shares of ownership in a corporation" },
                new AssetType { Id = 3, Name = "Real Estate", Description = "Property consisting of land and buildings" },
                new AssetType { Id = 4, Name = "Vehicle", Description = "Cars, motorcycles, boats, and other vehicles" },
                new AssetType { Id = 5, Name = "Commodity", Description = "Raw materials or primary agricultural products" },
                new AssetType { Id = 6, Name = "Collectibles", Description = "Items collected for their rarity, beauty, or historical significance" }
            );

            // Seed default user for migration
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "defaultuser", 
                    Email = "default@cryptoportfolio.com", 
                    PasswordHash = "default_hash_placeholder",
                    CreatedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                }
            );
        }
    }
}
