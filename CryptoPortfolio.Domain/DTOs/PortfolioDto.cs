using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoPortfolio.Domain.DTOs
{
    public class PortfolioDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal ProfitLossPercentage { get; set; }
        public decimal PortfolioPercentage { get; set; }
    }
}