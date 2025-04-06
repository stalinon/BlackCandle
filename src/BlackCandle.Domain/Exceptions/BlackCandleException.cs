namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Класс исключений проекта. Обозначает предвиденные исключения
/// </summary>
public class BlackCandleException : Exception
{
    /// <inheritdoc cref="BlackCandleException" />
    public BlackCandleException(string message)
        : base(message)
    {
    }

    /// <inheritdoc cref="BlackCandleException" />
    protected BlackCandleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
