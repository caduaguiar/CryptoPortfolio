using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoPortfolio.Domain.DTOs;

namespace CryptoPortfolio.Services
{
    public interface IAssetService
    {
        Task<List<AssetDto>> GetAssetsByPortfolioIdAsync(int portfolioId);
        Task<AssetDto?> GetAssetByIdAsync(int assetId);
        Task<AssetDto> CreateAssetAsync(CreateAssetDto dto);
        Task<AssetDto> UpdateAssetAsync(int assetId, CreateAssetDto dto);
        Task DeleteAssetAsync(int assetId);
        Task<List<AssetDto>> GetAssetsByTypeAsync(int assetTypeId);
        Task UpdateAssetCurrentValueAsync(int assetId, decimal currentValue);
        Task<decimal> CalculateAssetAveragePriceAsync(int assetId);
        Task<List<TransactionDto>> GetAssetTransactionHistoryAsync(int assetId);
    }
}
