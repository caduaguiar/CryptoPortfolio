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
        private readonly ICurrencyConversionService _currencyConversionService;

        public AssetService(CryptoDbContext context, ICurrencyConversionService currencyConversionService)
        {
            _context = context;
            _currencyConversionService = currencyConversionService;
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
                AcquisitionDate = DateTime.SpecifyKind(dto.AcquisitionDate, DateTimeKind.Utc),
                AcquisitionCost = dto.AcquisitionCost,
                CurrentValue = dto.CurrentValue,
                Currency = dto.Currency,
                Location = dto.Location,
                Notes = dto.Notes,
                IsCrypto = dto.IsCrypto,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                IsActive = true
            };

            // Handle currency conversion for non-USD currencies
            if (dto.Currency.ToUpper() != "USD")
            {
                try
                {
                    var exchangeRate = await _currencyConversionService.GetExchangeRateToUSDAsync(dto.Currency);
                    asset.ExchangeRateToUSD = exchangeRate;
                    asset.ExchangeRateLastUpdated = DateTime.UtcNow;
                    asset.AcquisitionCostUSD = dto.AcquisitionCost * exchangeRate;
                    
                    if (dto.CurrentValue.HasValue)
                    {
                        asset.CurrentValueUSD = dto.CurrentValue.Value * exchangeRate;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but don't fail the asset creation
                    // The USD values will be null and can be updated later
                    Console.WriteLine($"Warning: Failed to get exchange rate for {dto.Currency}: {ex.Message}");
                }
            }
            else
            {
                // For USD assets, set USD values to the same as original values
                asset.AcquisitionCostUSD = dto.AcquisitionCost;
                asset.CurrentValueUSD = dto.CurrentValue;
                asset.ExchangeRateToUSD = 1.0m;
                asset.ExchangeRateLastUpdated = DateTime.UtcNow;
            }

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
            asset.AcquisitionDate = DateTime.SpecifyKind(dto.AcquisitionDate, DateTimeKind.Utc);
            asset.AcquisitionCost = dto.AcquisitionCost;
            asset.CurrentValue = dto.CurrentValue;
            asset.Currency = dto.Currency;
            asset.Location = dto.Location;
            asset.Notes = dto.Notes;
            asset.IsCrypto = dto.IsCrypto;
            asset.LastUpdated = DateTime.UtcNow;

            // Update currency conversion if currency changed or exchange rate is stale
            if (dto.Currency.ToUpper() != "USD")
            {
                if (_currencyConversionService.IsExchangeRateStale(asset.ExchangeRateLastUpdated) || 
                    asset.ExchangeRateToUSD == null)
                {
                    try
                    {
                        var exchangeRate = await _currencyConversionService.GetExchangeRateToUSDAsync(dto.Currency);
                        asset.ExchangeRateToUSD = exchangeRate;
                        asset.ExchangeRateLastUpdated = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Failed to update exchange rate for {dto.Currency}: {ex.Message}");
                    }
                }

                // Update USD values using current exchange rate
                if (asset.ExchangeRateToUSD.HasValue)
                {
                    asset.AcquisitionCostUSD = dto.AcquisitionCost * asset.ExchangeRateToUSD.Value;
                    if (dto.CurrentValue.HasValue)
                    {
                        asset.CurrentValueUSD = dto.CurrentValue.Value * asset.ExchangeRateToUSD.Value;
                    }
                }
            }
            else
            {
                // For USD assets, set USD values to the same as original values
                asset.AcquisitionCostUSD = dto.AcquisitionCost;
                asset.CurrentValueUSD = dto.CurrentValue;
                asset.ExchangeRateToUSD = 1.0m;
                asset.ExchangeRateLastUpdated = DateTime.UtcNow;
            }

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
                IsActive = asset.IsActive,
                IsCrypto = asset.IsCrypto,
                AcquisitionCostUSD = asset.AcquisitionCostUSD,
                CurrentValueUSD = asset.CurrentValueUSD,
                ExchangeRateToUSD = asset.ExchangeRateToUSD,
                ExchangeRateLastUpdated = asset.ExchangeRateLastUpdated
            };
        }
    }
}
