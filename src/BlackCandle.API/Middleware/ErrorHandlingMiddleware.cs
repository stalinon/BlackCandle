using BlackCandle.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.API.Middleware;

/// <summary>
///     Глобальный middleware обработки исключений, не даёт приложению упасть.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    /// <inheritdoc cref="ErrorHandlingMiddleware" />
    public ErrorHandlingMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <inheritdoc cref="ErrorHandlingMiddleware" />
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EmptyPortfolioException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode)
    {
        _logger.LogError("Произошло необработанное исключение.", ex);

        var response = OperationResult<string>.Failure(ex.Message);

        var payload = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(payload);
    }
}