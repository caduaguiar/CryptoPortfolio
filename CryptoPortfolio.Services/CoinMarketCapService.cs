using System.Text.Json;
using CryptoPortfolio.Data;
using CryptoPortfolio.Domain.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CryptoPortfolio.Services
{
    public class CoinMarketCapService : ICoinMarketCapService
    {
        private readonly HttpClient _httpClient;
        private readonly CryptoDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public CoinMarketCapService(HttpClient httpClient, CryptoDbContext context, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _context = context;
            _configuration = configuration;
            _apiKey = _configuration["CoinMarketCap:ApiKey"] ?? throw new InvalidOperationException("CoinMarketCap API key not configured");

            _httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<decimal> GetCurrentPriceAsync(string symbol)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={symbol}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);

                var price = jsonDoc.RootElement
                    .GetProperty("data")
                    .GetProperty(symbol)
                    .GetProperty("quote")
                    .GetProperty("USD")
                    .GetProperty("price")
                    .GetDecimal();

                return price;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get price for {symbol}: {ex.Message}");
            }
        }

        public async Task<List<PriceAlertDto>> GetPriceAlertsAsync(decimal percentageThreshold = 10m, int days = 7)
        {
            try
            {
                var symbols = await _context.Cryptocurrencies.Select(c => c.Symbol).ToListAsync();
                var symbolsParam = string.Join(",", symbols);

                var response = await _httpClient.GetAsync($"https://pro-api.coinmarketcap.com/v1/cryptocurrency/quotes/latest?symbol={symbolsParam}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);

                var alerts = new List<PriceAlertDto>();
                var dataElement = jsonDoc.RootElement.GetProperty("data");

                foreach (var symbol in symbols)
                {
                    if (dataElement.TryGetProperty(symbol, out var cryptoElement))
                    {
                        var quote = cryptoElement.GetProperty("quote").GetProperty("USD");
                        var priceChange7d = quote.GetProperty("percent_change_7d").GetDecimal();

                        if (priceChange7d >= percentageThreshold)
                        {
                            alerts.Add(new PriceAlertDto
                            {
                                Symbol = symbol,
                                Name = cryptoElement.GetProperty("name").GetString() ?? "",
                                CurrentPrice = quote.GetProperty("price").GetDecimal(),
                                PriceChangePercentage7d = priceChange7d
                            });
                        }
                    }
                }

                return alerts;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get price alerts: {ex.Message}");
            }
        }
        
        public async Task UpdateAllPricesAsync()
        {
            var cryptocurrencies = await _context.Cryptocurrencies.ToListAsync();
            
            foreach (var crypto in cryptocurrencies)
            {
                try
                {
                    var currentPrice = await GetCurrentPriceAsync(crypto.Symbol);
                    crypto.CurrentPrice = currentPrice;
                    crypto.LastUpdated = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    // Log error but continue with other cryptocurrencies
                    Console.WriteLine($"Failed to update price for {crypto.Symbol}: {ex.Message}");
                }
            }
            
            await _context.SaveChangesAsync();
        }
    }
}