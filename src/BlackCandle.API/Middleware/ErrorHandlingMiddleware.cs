using System.Net;
using System.Text.Json;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.API.Middleware;

/// <summary>
///     Глобальный middleware обработки исключений, не даёт приложению упасть.
/// </summary>
/// <inheritdoc cref="ErrorHandlingMiddleware" />
public class ErrorHandlingMiddleware(RequestDelegate next, ILoggerService logger)
{
    /// <inheritdoc cref="ErrorHandlingMiddleware" />
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BlackCandleException ex)
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
        logger.LogError("Произошло необработанное исключение.", ex);

        var response = OperationResult<string>.Failure(ex.Message);

        var payload = JsonSerializer.Serialize(response);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(payload);
    }
}
