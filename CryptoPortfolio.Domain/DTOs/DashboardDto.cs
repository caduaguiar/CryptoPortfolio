using System;
using System.Collections.Generic;

namespace CryptoPortfolio.Domain.DTOs
{
    public class DashboardDto
    {
        public decimal TotalPortfolioValue { get; set; }
        public decimal TotalInvested { get; set; }
        public decimal TotalProfitLoss { get; set; }
        public decimal TotalProfitLossPercentage { get; set; }
        public List<AssetDto> Assets { get; set; } = new List<AssetDto>();
        public List<AssetAllocationDto> AssetAllocation { get; set; } = new List<AssetAllocationDto>();
        public List<PortfolioSummaryDto> Portfolios { get; set; } = new List<PortfolioSummaryDto>();
    }
    
    public class AssetAllocationDto
    {
        public string AssetTypeName { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
        public decimal Percentage { get; set; }
        public int AssetCount { get; set; }
    }
    
    public class PortfolioSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BaseCurrency { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
        public decimal TotalInvested { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal ProfitLossPercentage { get; set; }
        public int AssetCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
