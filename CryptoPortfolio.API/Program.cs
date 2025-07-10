using CryptoPortfolio.API.Middleware;
using CryptoPortfolio.Data;
using CryptoPortfolio.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Suppress default model state validation to use custom validation
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<CryptoDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("CryptoPortfolio.API")
        ));

// Services
builder.Services.AddHttpClient<ICoinMarketCapService, CoinMarketCapService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IAssetService, AssetService>();

// Currency conversion service with HttpClient
builder.Services.AddHttpClient<ICurrencyConversionService, CurrencyConversionService>();

// CORS - Enhanced configuration for multiple environments
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCorsPolicy", corsBuilder =>
    {
        corsBuilder
            .WithOrigins(
                "http://localhost:3000",    // React dev server
                "http://localhost:5000",    // API base URL
                "http://localhost:5123",    // Alternative API port
                "https://localhost:7213"    // HTTPS API port
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowedToAllowWildcardSubdomains();
    });

    options.AddPolicy("ProductionCorsPolicy", corsBuilder =>
    {
        corsBuilder
            .WithOrigins(
                builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? 
                new[] { "http://localhost:3000" }
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline - IMPORTANT: Order matters!

// 1. Global exception middleware should be first
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2. Development-specific middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevelopmentCorsPolicy");
}
else
{
    app.UseCors("ProductionCorsPolicy");
}

// 3. Request logging middleware for debugging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path} from {Origin}", 
        context.Request.Method, 
        context.Request.Path, 
        context.Request.Headers["Origin"].FirstOrDefault() ?? "No Origin");
    
    await next();
    
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

// 4. Standard ASP.NET Core middleware
app.UseHttpsRedirection();
app.UseAuthorization();

// 5. Map controllers
app.MapControllers();

// 6. Health check endpoint
app.MapGet("/health", () => new { 
    Status = "Healthy", 
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName 
});

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        context.Database.EnsureCreated();
        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error initializing database");
    }
}

app.Run();
