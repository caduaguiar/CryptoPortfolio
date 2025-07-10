using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CryptoPortfolio.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    }
}
