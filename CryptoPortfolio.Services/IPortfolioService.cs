using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoPortfolio.Domain.DTOs;

namespace CryptoPortfolio.Services
{
    public interface IPortfolioService
    {
        Task<DashboardDto> GetDashboardAsync(int? userId = null);
        Task<List<TransactionDto>> GetTransactionsAsync(int? portfolioId = null);
        Task<TransactionDto> AddTransactionAsync(CreateTransactionDto dto);
        Task DeleteTransactionAsync(int transactionId);
        Task<List<PortfolioSummaryDto>> GetPortfoliosAsync(int? userId = null);
        Task<PortfolioSummaryDto?> GetPortfolioByIdAsync(int portfolioId);
        Task UpdateAssetValuesAsync();
        Task<List<AssetAllocationDto>> GetAssetAllocationAsync(int? portfolioId = null);
    }
}
