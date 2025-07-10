using System.Text.Json;
using CryptoPortfolio.Domain.DTOs;
using CryptoPortfolio.Services;
using Microsoft.AspNetCore.Mvc;


namespace CryptoPortfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ILogger<PortfolioController> _logger;
        
        public PortfolioController(IPortfolioService portfolioService, ILogger<PortfolioController> logger)
        {
            _portfolioService = portfolioService;
            _logger = logger;
        }
        
        [HttpPost("transactions")]
        public async Task<ActionResult<TransactionDto>> AddTransaction([FromBody] CreateTransactionDto dto)
        {
            try
            {
                // Enhanced validation with detailed error messages
                var validationErrors = ValidateTransactionDto(dto);
                if (validationErrors.Any())
                {
                    var errorResponse = new ValidationErrorResponseDto
                    {
                        Message = "The transaction data contains validation errors. Please fix the issues below and try again.",
                        Errors = validationErrors,
                        RequestInfo = new Dictionary<string, object>
                        {
                        ["providedAmount"] = dto.Amount,
                        ["providedPricePerUnit"] = dto.PricePerUnit,
                        ["providedAssetId"] = dto.AssetId,
                        ["providedType"] = dto.TransactionType,
                        ["calculatedTotal"] = dto.TotalTransactionValue
                        }
                    };
                    
                    return BadRequest(errorResponse);
                }
                
                // Additional business logic validation
                if (dto.TransactionDate > DateTime.UtcNow)
                {
                    return BadRequest(new ErrorResponseDto
                    {
                        Message = "Transaction date validation failed",
                        FieldErrors = new List<FieldError>
                        {
                            new FieldError
                            {
                                Field = "transactionDate",
                                Error = "Transaction date cannot be in the future",
                                ProvidedValue = dto.TransactionDate,
                                Suggestion = $"Use a date up to {DateTime.UtcNow:yyyy-MM-dd}"
                            }
                        }
                    });
                }
                
                var transaction = await _portfolioService.AddTransactionAsync(dto);
                
                _logger.LogInformation("Transaction created successfully: {TransactionId} for {AssetName} - {Type} {Amount}",
                    transaction.Id, transaction.AssetName, transaction.TransactionTypeString, transaction.Amount);
                
                return CreatedAtAction(nameof(GetTransactions), new { id = transaction.Id }, transaction);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogWarning(jsonEx, "Invalid JSON in transaction request");
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Invalid JSON format in request",
                    Details = "The request contains malformed JSON. Common issues: using commas in numbers, missing quotes, extra commas.",
                    FieldErrors = new List<FieldError>
                    {
                        new FieldError
                        {
                            Field = "JSON Format",
                            Error = jsonEx.Message,
                            Suggestion = "Use decimal points (0.1) not commas (0,1). Remove thousand separators from numbers."
                        }
                    },
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid argument in transaction request");
                return BadRequest(new ErrorResponseDto
                {
                    Message = "Invalid transaction data",
                    Details = argEx.Message,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating transaction");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "An unexpected error occurred while creating the transaction",
                    Details = "Please try again or contact support if the problem persists",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }
        
        private List<FieldError> ValidateTransactionDto(CreateTransactionDto dto)
        {
            var errors = new List<FieldError>();
            
            // Asset ID validation
            if (dto.AssetId <= 0)
            {
                errors.Add(new FieldError
                {
                    Field = "assetId",
                    Error = "Asset ID is required and must be greater than zero",
                    ProvidedValue = dto.AssetId,
                    Suggestion = "Provide a valid asset ID"
                });
            }
            
            // Portfolio ID validation
            if (dto.PortfolioId <= 0)
            {
                errors.Add(new FieldError
                {
                    Field = "portfolioId",
                    Error = "Portfolio ID is required and must be greater than zero",
                    ProvidedValue = dto.PortfolioId,
                    Suggestion = "Provide a valid portfolio ID"
                });
            }
            
            // Amount validation
            if (dto.Amount <= 0)
            {
                errors.Add(new FieldError
                {
                    Field = "amount",
                    Error = "Amount must be greater than zero",
                    ProvidedValue = dto.Amount,
                    Suggestion = "Enter a positive number like 0.1 or 1.5"
                });
            }
            else if (dto.Amount < 0.00000001m)
            {
                errors.Add(new FieldError
                {
                    Field = "amount",
                    Error = "Amount is too small (minimum 0.00000001)",
                    ProvidedValue = dto.Amount,
                    Suggestion = "Use at least 0.00000001"
                });
            }
            
            // Price validation (optional for some transaction types)
            if (dto.PricePerUnit.HasValue)
            {
                if (dto.PricePerUnit.Value <= 0)
                {
                    errors.Add(new FieldError
                    {
                        Field = "pricePerUnit",
                        Error = "Price per unit must be greater than zero when provided",
                        ProvidedValue = dto.PricePerUnit.Value,
                        Suggestion = "Enter a positive price or leave empty for non-priced transactions"
                    });
                }
                else if (dto.PricePerUnit.Value > 10000000m)
                {
                    errors.Add(new FieldError
                    {
                        Field = "pricePerUnit",
                        Error = "Price per unit is too large (maximum $10,000,000)",
                        ProvidedValue = dto.PricePerUnit.Value,
                        Suggestion = "Use a price less than $10,000,000"
                    });
                }
            }
            
            // Total transaction value validation
            if (dto.TotalTransactionValue <= 0)
            {
                errors.Add(new FieldError
                {
                    Field = "totalTransactionValue",
                    Error = "Total transaction value must be greater than zero",
                    ProvidedValue = dto.TotalTransactionValue,
                    Suggestion = "Enter a positive total value"
                });
            }
            else if (dto.TotalTransactionValue > 100000000m) // 100 million USD limit
            {
                errors.Add(new FieldError
                {
                    Field = "totalTransactionValue",
                    Error = $"Transaction total (${dto.TotalTransactionValue:N2}) exceeds maximum limit of $100,000,000",
                    ProvidedValue = dto.TotalTransactionValue,
                    Suggestion = "Reduce the total transaction value"
                });
            }
            
            // Currency validation
            if (string.IsNullOrWhiteSpace(dto.TransactionCurrency))
            {
                errors.Add(new FieldError
                {
                    Field = "transactionCurrency",
                    Error = "Transaction currency is required",
                    ProvidedValue = dto.TransactionCurrency,
                    Suggestion = "Provide a valid currency code like 'USD', 'BRL', 'EUR'"
                });
            }
            else if (dto.TransactionCurrency.Length < 3 || dto.TransactionCurrency.Length > 10)
            {
                errors.Add(new FieldError
                {
                    Field = "transactionCurrency",
                    Error = "Transaction currency must be between 3 and 10 characters",
                    ProvidedValue = dto.TransactionCurrency,
                    Suggestion = "Use standard currency codes like 'USD', 'BRL', 'EUR'"
                });
            }
            
            return errors;
        }
        
        // Other controller methods remain the same...
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            try
            {
                var dashboard = await _portfolioService.GetDashboardAsync();
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard data");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to load dashboard data",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }
        
        [HttpGet("transactions")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactions()
        {
            try
            {
                var transactions = await _portfolioService.GetTransactionsAsync();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions");
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to load transactions",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }
        
        [HttpDelete("transactions/{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                await _portfolioService.DeleteTransactionAsync(id);
                _logger.LogInformation("Transaction deleted successfully: {TransactionId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction {TransactionId}", id);
                return StatusCode(500, new ErrorResponseDto
                {
                    Message = "Failed to delete transaction",
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }
    }
}
