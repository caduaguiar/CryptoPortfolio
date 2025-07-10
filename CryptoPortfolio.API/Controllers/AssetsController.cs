using CryptoPortfolio.Domain.DTOs;
using CryptoPortfolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptoPortfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(IAssetService assetService, ILogger<AssetsController> logger)
        {
            _assetService = assetService;
            _logger = logger;
        }

        [HttpGet("portfolio/{portfolioId}")]
        public async Task<ActionResult<List<AssetDto>>> GetAssetsByPortfolio(int portfolioId)
        {
            try
            {
                var assets = await _assetService.GetAssetsByPortfolioIdAsync(portfolioId);
                return Ok(assets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assets for portfolio {PortfolioId}", portfolioId);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to load assets",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssetDto>> GetAsset(int id)
        {
            try
            {
                var asset = await _assetService.GetAssetByIdAsync(id);
                if (asset == null)
                {
                    return NotFound(new ErrorResponseDto
                    {
                        Message = $"Asset with ID {id} not found",
                        TraceId = HttpContext.TraceIdentifier
                    });
                }
                return Ok(asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting asset {AssetId}", id);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to load asset",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<AssetDto>> CreateAsset([FromBody] CreateAssetDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var asset = await _assetService.CreateAssetAsync(dto);
                _logger.LogInformation("Asset created successfully: {AssetId} - {AssetName}", asset.Id, asset.Name);
                
                return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid argument in create asset request");
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Invalid asset data",
                    Details = argEx.Message,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating asset");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "An unexpected error occurred while creating the asset",
                    Details = "Please try again or contact support if the problem persists",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AssetDto>> UpdateAsset(int id, [FromBody] CreateAssetDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var asset = await _assetService.UpdateAssetAsync(id, dto);
                _logger.LogInformation("Asset updated successfully: {AssetId} - {AssetName}", asset.Id, asset.Name);
                
                return Ok(asset);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid argument in update asset request for asset {AssetId}", id);
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Invalid asset data",
                    Details = argEx.Message,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating asset {AssetId}", id);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "An unexpected error occurred while updating the asset",
                    Details = "Please try again or contact support if the problem persists",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            try
            {
                await _assetService.DeleteAssetAsync(id);
                _logger.LogInformation("Asset deleted successfully: {AssetId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting asset {AssetId}", id);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to delete asset",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet("type/{assetTypeId}")]
        public async Task<ActionResult<List<AssetDto>>> GetAssetsByType(int assetTypeId)
        {
            try
            {
                var assets = await _assetService.GetAssetsByTypeAsync(assetTypeId);
                return Ok(assets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting assets for type {AssetTypeId}", assetTypeId);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to load assets by type",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpPut("{id}/current-value")]
        public async Task<IActionResult> UpdateAssetCurrentValue(int id, [FromBody] decimal currentValue)
        {
            try
            {
                if (currentValue < 0)
                {
                    return BadRequest(new ErrorResponseDto
                    {
                        Message = "Current value cannot be negative",
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                await _assetService.UpdateAssetCurrentValueAsync(id, currentValue);
                _logger.LogInformation("Asset current value updated: {AssetId} - ${CurrentValue}", id, currentValue);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating current value for asset {AssetId}", id);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to update asset current value",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet("{id}/average-price")]
        public async Task<ActionResult<decimal>> GetAssetAveragePrice(int id)
        {
            try
            {
                var averagePrice = await _assetService.CalculateAssetAveragePriceAsync(id);
                return Ok(averagePrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average price for asset {AssetId}", id);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to calculate asset average price",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpGet("{id}/transactions")]
        public async Task<ActionResult<List<TransactionDto>>> GetAssetTransactionHistory(int id)
        {
            try
            {
                var transactions = await _assetService.GetAssetTransactionHistoryAsync(id);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction history for asset {AssetId}", id);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to load asset transaction history",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }
    }
}
