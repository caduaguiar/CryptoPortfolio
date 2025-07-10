using System;

namespace CryptoPortfolio.Domain.DTOs
{
    public class AssetDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public int AssetTypeId { get; set; }
        public string AssetTypeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Symbol { get; set; }
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public decimal AcquisitionCost { get; set; }
        public decimal? CurrentValue { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsCrypto { get; set; }
        
        // Currency conversion fields
        public decimal? AcquisitionCostUSD { get; set; }
        public decimal? CurrentValueUSD { get; set; }
        public decimal? ExchangeRateToUSD { get; set; }
        public DateTime? ExchangeRateLastUpdated { get; set; }
        
        // Calculated properties
        public decimal? ProfitLoss => CurrentValue.HasValue ? CurrentValue.Value - AcquisitionCost : null;
        public decimal? ProfitLossPercentage => AcquisitionCost > 0 && CurrentValue.HasValue ? 
            ((CurrentValue.Value - AcquisitionCost) / AcquisitionCost) * 100 : null;
        public decimal? CurrentPricePerUnit => Quantity > 0 && CurrentValue.HasValue ? 
            CurrentValue.Value / Quantity : null;
        public decimal AcquisitionPricePerUnit => Quantity > 0 ? AcquisitionCost / Quantity : 0;
    }
}
