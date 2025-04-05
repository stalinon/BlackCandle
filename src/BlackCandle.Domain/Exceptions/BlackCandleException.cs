namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Класс исключений проекта. Обозначает предвиденные исключения
/// </summary>
public abstract class BlackCandleException : Exception
{
    /// <inheritdoc cref="BlackCandleException" />
    protected BlackCandleException(string message)
        : base(message)
    {
    }

    /// <inheritdoc cref="BlackCandleException" />
    protected BlackCandleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
