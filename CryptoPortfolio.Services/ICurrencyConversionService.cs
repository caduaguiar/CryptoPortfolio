using System;
using System.Threading.Tasks;

namespace CryptoPortfolio.Services
{
    public interface ICurrencyConversionService
    {
        /// <summary>
        /// Get the current exchange rate from a source currency to USD
        /// </summary>
        /// <param name="fromCurrency">Source currency code (e.g., "BRL")</param>
        /// <returns>Exchange rate to USD</returns>
        Task<decimal> GetExchangeRateToUSDAsync(string fromCurrency);
        
        /// <summary>
        /// Convert an amount from source currency to USD
        /// </summary>
        /// <param name="amount">Amount in source currency</param>
        /// <param name="fromCurrency">Source currency code</param>
        /// <returns>Amount in USD</returns>
        Task<decimal> ConvertToUSDAsync(decimal amount, string fromCurrency);
        
        /// <summary>
        /// Convert an amount from USD to target currency
        /// </summary>
        /// <param name="amountUSD">Amount in USD</param>
        /// <param name="toCurrency">Target currency code</param>
        /// <returns>Amount in target currency</returns>
        Task<decimal> ConvertFromUSDAsync(decimal amountUSD, string toCurrency);
        
        /// <summary>
        /// Update exchange rates for all assets in the system
        /// </summary>
        Task UpdateAllAssetExchangeRatesAsync();
        
        /// <summary>
        /// Check if exchange rate data is stale and needs updating
        /// </summary>
        /// <param name="lastUpdated">Last update timestamp</param>
        /// <returns>True if data is stale</returns>
        bool IsExchangeRateStale(DateTime? lastUpdated);
    }
}
