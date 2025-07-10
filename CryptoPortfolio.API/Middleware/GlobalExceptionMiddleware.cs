using System.Net;
using System.Text.Json;
using CryptoPortfolio.Domain.DTOs;

namespace CryptoPortfolio.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogWarning(jsonEx, "JSON parsing error");
            await HandleJsonExceptionAsync(httpContext, jsonEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleJsonExceptionAsync(HttpContext context, JsonException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var response = new ErrorResponseDto
        {
            Message = "Invalid JSON format",
            Details = "The request contains malformed JSON",
            FieldErrors = new List<FieldError>
            {
                new FieldError
                {
                    Field = "JSON Parsing",
                    Error = exception.Message,
                    Suggestion =
                        "Check for: comma decimals (0,1), thousand separators (1,000), missing quotes, extra commas"
                }
            },
            TraceId = context.TraceIdentifier
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ErrorResponseDto
        {
            Message = "An error occurred while processing your request",
            Details = "Please try again or contact support if the problem persists",
            TraceId = context.TraceIdentifier
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}

