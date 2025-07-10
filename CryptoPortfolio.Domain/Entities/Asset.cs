using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CryptoPortfolio.Domain.Entities
{
    public class Asset
    {
        public int Id { get; set; }
        
        public int PortfolioId { get; set; }
        
        public int AssetTypeId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Symbol { get; set; }
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        public decimal Quantity { get; set; }
        
        [Required]
        public DateTime AcquisitionDate { get; set; }
        
        [Required]
        public decimal AcquisitionCost { get; set; }
        
        public decimal? CurrentValue { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string Currency { get; set; } = "USD";
        
        [MaxLength(255)]
        public string? Location { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual Portfolio Portfolio { get; set; } = null!;
        public virtual AssetType AssetType { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
