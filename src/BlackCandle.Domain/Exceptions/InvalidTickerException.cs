namespace BlackCandle.Domain.Exceptions;

/// <summary>
/// Выбрасывается, если тикер некорректен или отсутствует.
/// </summary>
public class InvalidTickerException : BlackCandleException
{
    /// <inheritdoc cref="InvalidTickerException" />
    public InvalidTickerException(string symbol)
        : base($"Некорректный тикер: {symbol}") { }
}