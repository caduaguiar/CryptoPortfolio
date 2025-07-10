using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CryptoPortfolio.Domain.Entities
{
    public class Portfolio
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string BaseCurrency { get; set; } = "USD";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
