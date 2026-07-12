namespace Api.Middleware;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var provided) && !string.IsNullOrWhiteSpace(provided)
            ? provided.ToString()
            : Guid.NewGuid().ToString();

        context.Response.Headers[HeaderName] = correlationId;

        using (logger.BeginScope("CorrelationId:{CorrelationId}", correlationId))
        {
            await _next(context);
        }
    }
}
