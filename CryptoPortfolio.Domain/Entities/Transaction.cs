using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CryptoPortfolio.Domain.Enums;

namespace CryptoPortfolio.Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        
        public int AssetId { get; set; }
        
        public int PortfolioId { get; set; }
        
        [Required]
        public TransactionType TransactionType { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        public decimal? PricePerUnit { get; set; }
        
        [Required]
        public decimal TotalTransactionValue { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string TransactionCurrency { get; set; } = "USD";
        
        [Required]
        public DateTime TransactionDate { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Asset Asset { get; set; } = null!;
        public virtual Portfolio Portfolio { get; set; } = null!;
    }
}
