using Application.Exceptions;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (PaymentFailedException exception)
        {
            await WriteError(context, StatusCodes.Status402PaymentRequired, "PaymentFailed", exception.Message);
        }
        catch (UnknownGatewayException exception)
        {
            await WriteError(context, StatusCodes.Status400BadRequest, "UnknownGateway", exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception processing {Path}", context.Request.Path);
            await WriteError(context, StatusCodes.Status500InternalServerError, "InternalError", "An unexpected error occurred.");
        }
    }

    private static Task WriteError(HttpContext context, int statusCode, string error, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { error, message });
    }
}
