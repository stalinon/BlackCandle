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
public class ErrorHandlingMiddleware : IDisposable
{
    private readonly RequestDelegate _next;
    private readonly IServiceScope _scope;
    private readonly ILoggerService _logger;

    /// <inheritdoc cref="ErrorHandlingMiddleware" />
    public ErrorHandlingMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _scope = serviceScopeFactory.CreateScope();
        _logger = _scope.ServiceProvider.GetRequiredService<ILoggerService>();
    }

    /// <inheritdoc cref="ErrorHandlingMiddleware" />
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
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

    /// <inheritdoc />
    public void Dispose() => _scope.Dispose();

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
