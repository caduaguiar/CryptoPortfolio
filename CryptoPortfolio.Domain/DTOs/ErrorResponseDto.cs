namespace CryptoPortfolio.Domain.DTOs;

public class ErrorResponseDto
{
    public string Message { get; set; } = string.Empty;
    public List<FieldError> FieldErrors { get; set; } = new();
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string TraceId { get; set; } = string.Empty; 
}
public class FieldError
{
    public string Field { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public object? ProvidedValue { get; set; }
    public string? Suggestion { get; set; }
}
    
public class ValidationErrorResponseDto
{
    public string Message { get; set; } = "Validation failed";
    public List<FieldError> Errors { get; set; } = new();
    public Dictionary<string, object> RequestInfo { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}