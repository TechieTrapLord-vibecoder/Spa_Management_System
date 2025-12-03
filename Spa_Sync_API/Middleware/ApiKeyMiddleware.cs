namespace Spa_Sync_API.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private const string API_KEY_HEADER = "X-API-Key";

    public ApiKeyMiddleware(
        RequestDelegate next, 
        IConfiguration configuration,
        ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow health check and status endpoints without API key
        if (context.Request.Path.StartsWithSegments("/api/sync/status") ||
            context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        // Check for API key in header
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var providedApiKey))
        {
            _logger.LogWarning("API request without API key from {IP}", 
                context.Connection.RemoteIpAddress);
            
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { Error = "API Key is required" });
            return;
        }

        var configuredApiKey = _configuration["ApiKey"];
        
        if (string.IsNullOrEmpty(configuredApiKey))
        {
            _logger.LogError("API Key not configured on server");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { Error = "Server configuration error" });
            return;
        }

        if (!configuredApiKey.Equals(providedApiKey))
        {
            _logger.LogWarning("Invalid API key attempt from {IP}", 
                context.Connection.RemoteIpAddress);
            
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { Error = "Invalid API Key" });
            return;
        }

        await _next(context);
    }
}

// Extension method for easy registration
public static class ApiKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyMiddleware>();
    }
}
