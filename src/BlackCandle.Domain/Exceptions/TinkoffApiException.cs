namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Исключение, вызванное ошибкой при запросе в Tinkoff API
/// </summary>
public sealed class TinkoffApiException : BlackCandleException
{
    /// <inheritdoc cref="TinkoffApiException" />
    public TinkoffApiException()
        : base("Ошибка при обращении в Tinkoff API")
    {
    }

    /// <inheritdoc cref="TinkoffApiException" />
    public TinkoffApiException(string message)
        : base(message)
    {
    }
}
