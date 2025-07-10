using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CryptoPortfolio.Data;
using CryptoPortfolio.Domain.Entities;

namespace CryptoPortfolio.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyConversionService> _logger;
        private readonly CryptoDbContext _context;
        
        // Using exchangerate-api.com (free tier: 1500 requests/month)
        private const string ExchangeRateApiUrl = "https://api.exchangerate-api.com/v4/latest/USD";
        
        public CurrencyConversionService(
            HttpClient httpClient, 
            ILogger<CurrencyConversionService> logger,
            CryptoDbContext context)
        {
            _httpClient = httpClient;
            _logger = logger;
            _context = context;
        }

        public async Task<decimal> GetExchangeRateToUSDAsync(string fromCurrency)
        {
            if (fromCurrency.ToUpper() == "USD")
                return 1.0m;

            try
            {
                _logger.LogInformation($"Fetching exchange rate for {fromCurrency} to USD");
                
                var response = await _httpClient.GetStringAsync(ExchangeRateApiUrl);
                _logger.LogInformation($"Raw API response length: {response.Length}");
                _logger.LogInformation($"First 200 chars of response: {response.Substring(0, Math.Min(200, response.Length))}");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var exchangeData = JsonSerializer.Deserialize<ExchangeRateResponse>(response, options);
                _logger.LogInformation($"Deserialized data - Base: {exchangeData?.Base}, Rates count: {exchangeData?.Rates?.Count ?? 0}");
                
                if (exchangeData?.Rates != null)
                {
                    _logger.LogInformation($"Looking for currency: '{fromCurrency}' in {exchangeData.Rates.Count} rates");
                    _logger.LogInformation($"Available currencies: {string.Join(", ", exchangeData.Rates.Keys.Take(20))}");
                    
                    // Try to find the currency (case insensitive)
                    var currencyKey = exchangeData.Rates.Keys.FirstOrDefault(k => 
                        string.Equals(k, fromCurrency, StringComparison.OrdinalIgnoreCase));
                    
                    _logger.LogInformation($"Currency key found: '{currencyKey}'");
                    
                    if (currencyKey != null)
                    {
                        // The API returns rates from USD to other currencies
                        // To get rate TO USD, we need to take the inverse
                        var rateFromUSD = exchangeData.Rates[currencyKey];
                        var rateToUSD = 1.0m / rateFromUSD;
                        
                        _logger.LogInformation($"Exchange rate {fromCurrency}/USD: {rateToUSD:F6} (1 USD = {rateFromUSD} {fromCurrency})");
                        return rateToUSD;
                    }
                }
                
                _logger.LogWarning($"Currency {fromCurrency} not found in exchange rate data. Available currencies: {string.Join(", ", exchangeData?.Rates?.Keys?.Take(10) ?? new string[0])}");
                
                // Ultimate fallback for BRL (using current approximate rate)
                if (fromCurrency.ToUpper() == "BRL")
                {
                    _logger.LogWarning("Using fallback BRL/USD rate of 0.18");
                    return 0.18m; // Approximate BRL to USD rate
                }
                
                return 1.0m; // Fallback to 1:1 if currency not found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching exchange rate for {fromCurrency}");
                
                // Fallback: try to get last known rate from database
                var lastKnownRate = await GetLastKnownExchangeRateAsync(fromCurrency);
                if (lastKnownRate.HasValue)
                {
                    _logger.LogInformation($"Using last known exchange rate for {fromCurrency}: {lastKnownRate.Value:F6}");
                    return lastKnownRate.Value;
                }
                
                // Ultimate fallback for BRL (approximate rate)
                if (fromCurrency.ToUpper() == "BRL")
                {
                    _logger.LogWarning("Using fallback BRL/USD rate of 0.20");
                    return 0.20m; // Approximate BRL to USD rate
                }
                
                return 1.0m;
            }
        }

        public async Task<decimal> ConvertToUSDAsync(decimal amount, string fromCurrency)
        {
            if (fromCurrency.ToUpper() == "USD")
                return amount;
                
            var exchangeRate = await GetExchangeRateToUSDAsync(fromCurrency);
            return amount * exchangeRate;
        }

        public async Task<decimal> ConvertFromUSDAsync(decimal amountUSD, string toCurrency)
        {
            if (toCurrency.ToUpper() == "USD")
                return amountUSD;
                
            var exchangeRateToUSD = await GetExchangeRateToUSDAsync(toCurrency);
            var exchangeRateFromUSD = 1.0m / exchangeRateToUSD;
            return amountUSD * exchangeRateFromUSD;
        }

        public async Task UpdateAllAssetExchangeRatesAsync()
        {
            _logger.LogInformation("Starting to update exchange rates for all assets");
            
            var assetsWithNonUSDCurrency = await _context.Assets
                .Where(a => a.IsActive && a.Currency.ToUpper() != "USD")
                .ToListAsync();
                
            foreach (var asset in assetsWithNonUSDCurrency)
            {
                try
                {
                    var exchangeRate = await GetExchangeRateToUSDAsync(asset.Currency);
                    
                    // Update USD equivalent values
                    asset.ExchangeRateToUSD = exchangeRate;
                    asset.ExchangeRateLastUpdated = DateTime.UtcNow;
                    asset.AcquisitionCostUSD = asset.AcquisitionCost * exchangeRate;
                    
                    if (asset.CurrentValue.HasValue)
                    {
                        asset.CurrentValueUSD = asset.CurrentValue.Value * exchangeRate;
                    }
                    
                    asset.LastUpdated = DateTime.UtcNow;
                    
                    _logger.LogInformation($"Updated exchange rate for asset {asset.Name} ({asset.Currency}): {exchangeRate:F6}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to update exchange rate for asset {asset.Name} ({asset.Currency})");
                }
            }
            
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Completed updating exchange rates for {assetsWithNonUSDCurrency.Count} assets");
        }

        public bool IsExchangeRateStale(DateTime? lastUpdated)
        {
            if (!lastUpdated.HasValue)
                return true;
                
            // Consider exchange rate stale if it's older than 24 hours
            return DateTime.UtcNow - lastUpdated.Value > TimeSpan.FromHours(24);
        }
        
        private async Task<decimal?> GetLastKnownExchangeRateAsync(string currency)
        {
            try
            {
                var lastAssetWithRate = await _context.Assets
                    .Where(a => a.Currency.ToUpper() == currency.ToUpper() && a.ExchangeRateToUSD.HasValue)
                    .OrderByDescending(a => a.ExchangeRateLastUpdated)
                    .FirstOrDefaultAsync();
                    
                return lastAssetWithRate?.ExchangeRateToUSD;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving last known exchange rate for {currency}");
                return null;
            }
        }
    }

    // Response model for exchange rate API
    public class ExchangeRateResponse
    {
        [JsonPropertyName("base")]
        public string? Base { get; set; }
        
        [JsonPropertyName("date")]
        public string? Date { get; set; }
        
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal>? Rates { get; set; }
    }
}
