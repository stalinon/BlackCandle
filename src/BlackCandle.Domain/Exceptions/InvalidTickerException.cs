namespace BlackCandle.Domain.Exceptions;

/// <summary>
/// Выбрасывается, если тикер некорректен или отсутствует.
/// </summary>
/// <inheritdoc cref="InvalidTickerException" />
public class InvalidTickerException(string symbol) : BlackCandleException($"Некорректный тикер: {symbol}")
{
}
