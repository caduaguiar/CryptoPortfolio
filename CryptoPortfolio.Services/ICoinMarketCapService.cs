using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoPortfolio.Domain.DTOs;

namespace CryptoPortfolio.Services
{
    public interface ICoinMarketCapService
    {
        Task<decimal> GetCurrentPriceAsync(string symbol);
        Task<List<PriceAlertDto>> GetPriceAlertsAsync(decimal percentageThreshold = 10m, int days = 7);
        Task UpdateAllPricesAsync();
    }
}