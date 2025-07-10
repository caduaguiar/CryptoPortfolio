using Microsoft.AspNetCore.Mvc;

namespace CryptoPortfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetHealth()
        {
            _logger.LogInformation("Health check requested from {Origin}", 
                Request.Headers["Origin"].FirstOrDefault() ?? "No Origin");

            var healthInfo = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Version = "1.0.0",
                Server = new
                {
                    MachineName = Environment.MachineName,
                    ProcessId = Environment.ProcessId,
                    WorkingSet = Environment.WorkingSet
                },
                Request = new
                {
                    Origin = Request.Headers["Origin"].FirstOrDefault(),
                    UserAgent = Request.Headers["User-Agent"].FirstOrDefault(),
                    Host = Request.Host.ToString(),
                    Scheme = Request.Scheme,
                    Path = Request.Path,
                    Method = Request.Method
                }
            };

            return Ok(healthInfo);
        }

        [HttpGet("cors-test")]
        public IActionResult CorsTest()
        {
            _logger.LogInformation("CORS test requested from {Origin}", 
                Request.Headers["Origin"].FirstOrDefault() ?? "No Origin");

            return Ok(new
            {
                Message = "CORS is working correctly!",
                Origin = Request.Headers["Origin"].FirstOrDefault(),
                Timestamp = DateTime.UtcNow,
                Headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
            });
        }
    }
}
