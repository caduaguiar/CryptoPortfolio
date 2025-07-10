using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoPortfolio.Domain.DTOs;
using CryptoPortfolio.Services;
using Microsoft.AspNetCore.Mvc; 

namespace CryptoPortfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly ICoinMarketCapService _coinMarketCapService;
        
        public AlertsController(ICoinMarketCapService coinMarketCapService)
        {
            _coinMarketCapService = coinMarketCapService;
        }
        
        [HttpGet]
        public async Task<ActionResult<List<PriceAlertDto>>> GetPriceAlerts(
            [FromQuery] decimal threshold = 10m, 
            [FromQuery] int days = 7)
        {
            try
            {
                var alerts = await _coinMarketCapService.GetPriceAlertsAsync(threshold, days);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("update-prices")]
        public async Task<IActionResult> UpdatePrices()
        {
            try
            {
                await _coinMarketCapService.UpdateAllPricesAsync();
                return Ok(new { message = "Prices updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}