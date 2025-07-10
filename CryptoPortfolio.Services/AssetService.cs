using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoPortfolio.Data;
using CryptoPortfolio.Domain.DTOs;
using CryptoPortfolio.Domain.Entities;
using CryptoPortfolio.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CryptoPortfolio.Services
{
    public class AssetService : IAssetService
    {
        private readonly CryptoDbContext _context;

        public AssetService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<List<AssetDto>> GetAssetsByPortfolioIdAsync(int portfolioId)
        {
            var assets = await _context.Assets
                .Include(a => a.AssetType)
                .Where(a => a.PortfolioId == portfolioId && a.IsActive)
                .OrderBy(a => a.AssetType.Name)
                .ThenBy(a => a.Name)
                .ToListAsync();

            return assets.Select(MapToDto).ToList();
        }

        public async Task<AssetDto?> GetAssetByIdAsync(int assetId)
        {
            var asset = await _context.Assets
                .Include(a => a.AssetType)
                .FirstOrDefaultAsync(a => a.Id == assetId);

            return asset != null ? MapToDto(asset) : null;
        }

        public async Task<AssetDto> CreateAssetAsync(CreateAssetDto dto)
        {
            var asset = new Asset
            {
                PortfolioId = dto.PortfolioId,
                AssetTypeId = dto.AssetTypeId,
                Name = dto.Name,
                Symbol = dto.Symbol,
                Description = dto.Description,
                Quantity = dto.Quantity,
                AcquisitionDate = dto.AcquisitionDate,
                AcquisitionCost = dto.AcquisitionCost,
                CurrentValue = dto.CurrentValue,
                Currency = dto.Currency,
                Location = dto.Location,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsActive = true
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            return await GetAssetByIdAsync(asset.Id) ?? throw new InvalidOperationException("Failed to retrieve created asset");
        }

        public async Task<AssetDto> UpdateAssetAsync(int assetId, CreateAssetDto dto)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset == null)
                throw new ArgumentException($"Asset with ID {assetId} not found");

            asset.PortfolioId = dto.PortfolioId;
            asset.AssetTypeId = dto.AssetTypeId;
            asset.Name = dto.Name;
            asset.Symbol = dto.Symbol;
            asset.Description = dto.Description;
            asset.Quantity = dto.Quantity;
            asset.AcquisitionDate = dto.AcquisitionDate;
            asset.AcquisitionCost = dto.AcquisitionCost;
            asset.CurrentValue = dto.CurrentValue;
            asset.Currency = dto.Currency;
            asset.Location = dto.Location;
            asset.Notes = dto.Notes;
            asset.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetAssetByIdAsync(assetId) ?? throw new InvalidOperationException("Failed to retrieve updated asset");
        }

        public async Task DeleteAssetAsync(int assetId)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset != null)
            {
                asset.IsActive = false;
                asset.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<AssetDto>> GetAssetsByTypeAsync(int assetTypeId)
        {
            var assets = await _context.Assets
                .Include(a => a.AssetType)
                .Where(a => a.AssetTypeId == assetTypeId && a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return assets.Select(MapToDto).ToList();
        }

        public async Task UpdateAssetCurrentValueAsync(int assetId, decimal currentValue)
        {
            var asset = await _context.Assets.FindAsync(assetId);
            if (asset != null)
            {
                asset.CurrentValue = currentValue;
                asset.LastUpdated = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> CalculateAssetAveragePriceAsync(int assetId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AssetId == assetId && 
                           (t.TransactionType == TransactionType.Buy || t.TransactionType == TransactionType.Deposit))
                .ToListAsync();

            if (!transactions.Any())
                return 0;

            var totalAmount = transactions.Sum(t => t.Amount);
            var totalValue = transactions.Sum(t => t.TotalTransactionValue);

            return totalAmount > 0 ? totalValue / totalAmount : 0;
        }

        public async Task<List<TransactionDto>> GetAssetTransactionHistoryAsync(int assetId)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Asset)
                .ThenInclude(a => a.AssetType)
                .Where(t => t.AssetId == assetId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                AssetId = t.AssetId,
                PortfolioId = t.PortfolioId,
                AssetName = t.Asset.Name,
                AssetSymbol = t.Asset.Symbol,
                AssetTypeName = t.Asset.AssetType.Name,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                PricePerUnit = t.PricePerUnit,
                TotalTransactionValue = t.TotalTransactionValue,
                TransactionCurrency = t.TransactionCurrency,
                TransactionDate = t.TransactionDate,
                Notes = t.Notes,
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        private static AssetDto MapToDto(Asset asset)
        {
            return new AssetDto
            {
                Id = asset.Id,
                PortfolioId = asset.PortfolioId,
                AssetTypeId = asset.AssetTypeId,
                AssetTypeName = asset.AssetType.Name,
                Name = asset.Name,
                Symbol = asset.Symbol,
                Description = asset.Description,
                Quantity = asset.Quantity,
                AcquisitionDate = asset.AcquisitionDate,
                AcquisitionCost = asset.AcquisitionCost,
                CurrentValue = asset.CurrentValue,
                Currency = asset.Currency,
                Location = asset.Location,
                Notes = asset.Notes,
                LastUpdated = asset.LastUpdated,
                CreatedAt = asset.CreatedAt,
                IsActive = asset.IsActive
            };
        }
    }
}
