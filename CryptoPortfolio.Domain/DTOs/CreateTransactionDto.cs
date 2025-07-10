using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CryptoPortfolio.Domain.Enums;

namespace CryptoPortfolio.Domain.DTOs
{
    public class CreateTransactionDto
    {
        [Required(ErrorMessage = "Asset ID is required")]
        public int AssetId { get; set; }
        
        [Required(ErrorMessage = "Portfolio ID is required")]
        public int PortfolioId { get; set; }
        
        [Required(ErrorMessage = "Transaction type is required")]
        public TransactionType TransactionType { get; set; }
        
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per unit must be greater than zero")]
        public decimal? PricePerUnit { get; set; }
        
        [Required(ErrorMessage = "Total transaction value is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total transaction value must be greater than zero")]
        public decimal TotalTransactionValue { get; set; }
        
        [Required(ErrorMessage = "Transaction currency is required")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Transaction currency must be between 3 and 10 characters")]
        public string TransactionCurrency { get; set; } = "USD";
        
        [Required(ErrorMessage = "Transaction date is required")]
        public DateTime TransactionDate { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notes must be 1000 characters or less")]
        public string? Notes { get; set; }
        
        // Read-only calculated property
        [JsonIgnore]
        public decimal? CalculatedPricePerUnit => Amount > 0 ? TotalTransactionValue / Amount : null;
    }
}
