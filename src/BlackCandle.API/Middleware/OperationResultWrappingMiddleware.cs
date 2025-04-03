using System.Text.Json;

namespace BlackCandle.API.Middleware;

/// <summary>
///     Мидлварь для автоматического оборачивания ответов в общий ValueObject
/// </summary>
public class OperationResultWrappingMiddleware
{
    private readonly RequestDelegate _next;

    /// <inheritdoc cref="OperationResultWrappingMiddleware" />
    public OperationResultWrappingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <inheritdoc cref="OperationResultWrappingMiddleware" />
    public async Task Invoke(HttpContext context)
    {
        var originalBody = context.Response.Body;

        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        if (context.Response.StatusCode == 200 && 
            context.Response.ContentType?.Contains("application/json") == true)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

            var wrapped = JsonSerializer.Serialize(new
            {
                isSuccess = true,
                data = JsonSerializer.Deserialize<object>(responseBody)
            });

            context.Response.Body = originalBody;
            await context.Response.WriteAsync(wrapped);
        }
        else
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBody);
        }
    }
}