using System;
using System.ComponentModel.DataAnnotations;

namespace CryptoPortfolio.Domain.DTOs
{
    public class CreateAssetDto
    {
        [Required(ErrorMessage = "Portfolio ID is required")]
        public int PortfolioId { get; set; }
        
        [Required(ErrorMessage = "Asset type ID is required")]
        public int AssetTypeId { get; set; }
        
        [Required(ErrorMessage = "Asset name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Asset name must be between 1 and 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(20, ErrorMessage = "Symbol must be 20 characters or less")]
        public string? Symbol { get; set; }
        
        [StringLength(1000, ErrorMessage = "Description must be 1000 characters or less")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public decimal Quantity { get; set; }
        
        [Required(ErrorMessage = "Acquisition date is required")]
        public DateTime AcquisitionDate { get; set; }
        
        [Required(ErrorMessage = "Acquisition cost is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Acquisition cost must be greater than zero")]
        public decimal AcquisitionCost { get; set; }
        
        public decimal? CurrentValue { get; set; }
        
        [Required(ErrorMessage = "Currency is required")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Currency must be between 3 and 10 characters")]
        public string Currency { get; set; } = "USD";
        
        [StringLength(255, ErrorMessage = "Location must be 255 characters or less")]
        public string? Location { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notes must be 1000 characters or less")]
        public string? Notes { get; set; }
        
        public bool IsCrypto { get; set; } = false;
    }
}
