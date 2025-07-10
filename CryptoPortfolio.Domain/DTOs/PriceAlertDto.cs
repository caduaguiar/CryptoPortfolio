using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoPortfolio.Domain.DTOs
{
    public class PriceAlertDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal PriceChange7d { get; set; }
        public decimal PriceChangePercentage7d { get; set; }
    }
}