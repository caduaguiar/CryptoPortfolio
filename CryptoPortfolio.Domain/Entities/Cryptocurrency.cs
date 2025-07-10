using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoPortfolio.Domain.Entities
{
    public class Cryptocurrency
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } // BTC, ETH, etc.
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Bitcoin, Ethereum, etc.
        
        public decimal CurrentPrice { get; set; }
        
        public DateTime LastUpdated { get; set; }
        
        public string? CoinMarketCapId { get; set; }
        
        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        
    }
}