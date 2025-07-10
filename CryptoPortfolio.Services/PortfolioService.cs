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
    public class PortfolioService : IPortfolioService
    {
        private readonly CryptoDbContext _context;
        private readonly ICoinMarketCapService _coinMarketCapService;

        public PortfolioService(CryptoDbContext context, ICoinMarketCapService coinMarketCapService)
        {
            _context = context;
            _coinMarketCapService = coinMarketCapService;
        }

        public async Task<DashboardDto> GetDashboardAsync(int? userId = null)
        {
            var query = _context.Assets
                .Include(a => a.AssetType)
                .Include(a => a.Portfolio)
                .Where(a => a.IsActive);

            if (userId.HasValue)
            {
                query = query.Where(a => a.Portfolio.UserId == userId.Value);
            }

            var assets = await query.ToListAsync();

            var assetDtos = assets.Select(a => new AssetDto
            {
                Id = a.Id,
                PortfolioId = a.PortfolioId,
                AssetTypeId = a.AssetTypeId,
                AssetTypeName = a.AssetType.Name,
                Name = a.Name,
                Symbol = a.Symbol,
                Description = a.Description,
                Quantity = a.Quantity,
                AcquisitionDate = a.AcquisitionDate,
                AcquisitionCost = a.AcquisitionCost,
                CurrentValue = a.CurrentValue,
                Currency = a.Currency,
                Location = a.Location,
                Notes = a.Notes,
                LastUpdated = a.LastUpdated,
                CreatedAt = a.CreatedAt,
                IsActive = a.IsActive
            }).ToList();

            var totalPortfolioValue = assetDtos.Where(a => a.CurrentValue.HasValue).Sum(a => a.CurrentValue!.Value);
            var totalInvested = assetDtos.Sum(a => a.AcquisitionCost);
            var totalProfitLoss = totalPortfolioValue - totalInvested;
            var totalProfitLossPercentage = totalInvested > 0 ? (totalProfitLoss / totalInvested) * 100 : 0;

            // Calculate asset allocation
            var assetAllocation = assetDtos
                .GroupBy(a => a.AssetTypeName)
                .Select(g => new AssetAllocationDto
                {
                    AssetTypeName = g.Key,
                    TotalValue = g.Where(a => a.CurrentValue.HasValue).Sum(a => a.CurrentValue!.Value),
                    AssetCount = g.Count(),
                    Percentage = totalPortfolioValue > 0 ? 
                        (g.Where(a => a.CurrentValue.HasValue).Sum(a => a.CurrentValue!.Value) / totalPortfolioValue) * 100 : 0
                })
                .OrderByDescending(a => a.TotalValue)
                .ToList();

            // Get portfolio summaries
            var portfolios = await GetPortfoliosAsync(userId);

            return new DashboardDto
            {
                TotalPortfolioValue = totalPortfolioValue,
                TotalInvested = totalInvested,
                TotalProfitLoss = totalProfitLoss,
                TotalProfitLossPercentage = totalProfitLossPercentage,
                Assets = assetDtos.OrderByDescending(a => a.CurrentValue ?? a.AcquisitionCost).ToList(),
                AssetAllocation = assetAllocation,
                Portfolios = portfolios
            };
        }

        public async Task<List<TransactionDto>> GetTransactionsAsync(int? portfolioId = null)
        {
            var query = _context.Transactions
                .Include(t => t.Asset)
                .ThenInclude(a => a.AssetType)
                .AsQueryable();

            if (portfolioId.HasValue)
            {
                query = query.Where(t => t.PortfolioId == portfolioId.Value);
            }

            var transactions = await query
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

        public async Task<TransactionDto> AddTransactionAsync(CreateTransactionDto dto)
        {
            // Validate that the asset exists and belongs to the specified portfolio
            var asset = await _context.Assets
                .Include(a => a.AssetType)
                .FirstOrDefaultAsync(a => a.Id == dto.AssetId && a.PortfolioId == dto.PortfolioId);

            if (asset == null)
                throw new ArgumentException($"Asset with ID {dto.AssetId} not found in portfolio {dto.PortfolioId}");

            var transaction = new Transaction
            {
                AssetId = dto.AssetId,
                PortfolioId = dto.PortfolioId,
                TransactionType = dto.TransactionType,
                Amount = dto.Amount,
                PricePerUnit = dto.PricePerUnit,
                TotalTransactionValue = dto.TotalTransactionValue,
                TransactionCurrency = dto.TransactionCurrency,
                TransactionDate = dto.TransactionDate,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Update asset quantity and current value based on transaction
            await UpdateAssetFromTransactionAsync(asset, transaction);

            return new TransactionDto
            {
                Id = transaction.Id,
                AssetId = transaction.AssetId,
                PortfolioId = transaction.PortfolioId,
                AssetName = asset.Name,
                AssetSymbol = asset.Symbol,
                AssetTypeName = asset.AssetType.Name,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                PricePerUnit = transaction.PricePerUnit,
                TotalTransactionValue = transaction.TotalTransactionValue,
                TransactionCurrency = transaction.TransactionCurrency,
                TransactionDate = transaction.TransactionDate,
                Notes = transaction.Notes,
                CreatedAt = transaction.CreatedAt
            };
        }

        public async Task DeleteTransactionAsync(int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Asset)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();

                // Recalculate asset values after transaction deletion
                await RecalculateAssetValuesAsync(transaction.Asset);
            }
        }

        public async Task<List<PortfolioSummaryDto>> GetPortfoliosAsync(int? userId = null)
        {
            var query = _context.Portfolios
                .Include(p => p.Assets.Where(a => a.IsActive))
                .ThenInclude(a => a.AssetType)
                .Where(p => p.IsActive);

            if (userId.HasValue)
            {
                query = query.Where(p => p.UserId == userId.Value);
            }

            var portfolios = await query.ToListAsync();

            return portfolios.Select(p =>
            {
                var totalValue = p.Assets.Where(a => a.CurrentValue.HasValue).Sum(a => a.CurrentValue!.Value);
                var totalInvested = p.Assets.Sum(a => a.AcquisitionCost);
                var profitLoss = totalValue - totalInvested;
                var profitLossPercentage = totalInvested > 0 ? (profitLoss / totalInvested) * 100 : 0;

                return new PortfolioSummaryDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    BaseCurrency = p.BaseCurrency,
                    TotalValue = totalValue,
                    TotalInvested = totalInvested,
                    ProfitLoss = profitLoss,
                    ProfitLossPercentage = profitLossPercentage,
                    AssetCount = p.Assets.Count,
                    LastUpdated = p.LastUpdated
                };
            }).ToList();
        }

        public async Task<PortfolioSummaryDto?> GetPortfolioByIdAsync(int portfolioId)
        {
            var portfolios = await GetPortfoliosAsync();
            return portfolios.FirstOrDefault(p => p.Id == portfolioId);
        }

        public async Task UpdateAssetValuesAsync()
        {
            // Update cryptocurrency prices
            var cryptoAssets = await _context.Assets
                .Include(a => a.AssetType)
                .Where(a => a.AssetType.Name == "Cryptocurrency" && a.IsActive && !string.IsNullOrEmpty(a.Symbol))
                .ToListAsync();

            foreach (var asset in cryptoAssets)
            {
                try
                {
                    // This would integrate with your existing CoinMarketCap service
                    // For now, we'll skip the actual price update
                    // var currentPrice = await _coinMarketCapService.GetCurrentPriceAsync(asset.Symbol);
                    // asset.CurrentValue = asset.Quantity * currentPrice;
                    // asset.LastUpdated = DateTime.UtcNow;
                }
                catch (Exception)
                {
                    // Log error but continue with other assets
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<AssetAllocationDto>> GetAssetAllocationAsync(int? portfolioId = null)
        {
            var query = _context.Assets
                .Include(a => a.AssetType)
                .Where(a => a.IsActive);

            if (portfolioId.HasValue)
            {
                query = query.Where(a => a.PortfolioId == portfolioId.Value);
            }

            var assets = await query.ToListAsync();
            var totalValue = assets.Where(a => a.CurrentValue.HasValue).Sum(a => a.CurrentValue!.Value);

            return assets
                .GroupBy(a => a.AssetType.Name)
                .Select(g => new AssetAllocationDto
                {
                    AssetTypeName = g.Key,
                    TotalValue = g.Where(a => a.CurrentValue.HasValue).Sum(a => a.CurrentValue!.Value),
                    AssetCount = g.Count(),
                    Percentage = totalValue > 0 ? 
                        (g.Where(a => a.CurrentValue.HasValue).Sum(a => a.CurrentValue!.Value) / totalValue) * 100 : 0
                })
                .OrderByDescending(a => a.TotalValue)
                .ToList();
        }

        private async Task UpdateAssetFromTransactionAsync(Asset asset, Transaction transaction)
        {
            // Recalculate asset values based on all transactions
            await RecalculateAssetValuesAsync(asset);
        }

        private async Task RecalculateAssetValuesAsync(Asset asset)
        {
            var transactions = await _context.Transactions
                .Where(t => t.AssetId == asset.Id)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            decimal totalQuantity = 0;
            decimal totalCost = 0;

            foreach (var transaction in transactions)
            {
                switch (transaction.TransactionType)
                {
                    case TransactionType.Buy:
                    case TransactionType.Deposit:
                        totalQuantity += transaction.Amount;
                        totalCost += transaction.TotalTransactionValue;
                        break;
                    case TransactionType.Sell:
                    case TransactionType.Withdrawal:
                        totalQuantity -= transaction.Amount;
                        // For sells, we don't subtract from total cost to maintain average price calculation
                        break;
                    case TransactionType.Fee:
                    case TransactionType.Maintenance:
                        totalCost += transaction.TotalTransactionValue;
                        break;
                    case TransactionType.Dividend:
                        // Dividends increase value but not quantity
                        break;
                    case TransactionType.Improvement:
                        totalCost += transaction.TotalTransactionValue;
                        break;
                    case TransactionType.Valuation:
                        // Valuation updates current value but doesn't affect quantity or cost
                        if (transaction.PricePerUnit.HasValue)
                        {
                            asset.CurrentValue = totalQuantity * transaction.PricePerUnit.Value;
                        }
                        break;
                }
            }

            asset.Quantity = totalQuantity;
            asset.AcquisitionCost = totalCost;
            asset.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
