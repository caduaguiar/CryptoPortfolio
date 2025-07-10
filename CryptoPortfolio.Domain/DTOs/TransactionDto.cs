using System;
using CryptoPortfolio.Domain.Enums;

namespace CryptoPortfolio.Domain.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public int PortfolioId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string? AssetSymbol { get; set; }
        public string AssetTypeName { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }
        public string TransactionTypeString => TransactionType.ToString();
        public decimal Amount { get; set; }
        public decimal? PricePerUnit { get; set; }
        public decimal TotalTransactionValue { get; set; }
        public string TransactionCurrency { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
