using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CryptoPortfolio.Domain.Entities
{
    public class AssetType
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        // Navigation properties
        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
