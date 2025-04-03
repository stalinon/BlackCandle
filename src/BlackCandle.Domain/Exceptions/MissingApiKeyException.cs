namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Выбрасывается, если не найден API-ключ.
/// </summary>
public class MissingApiKeyException : Exception
{
    /// <inheritdoc cref="MissingApiKeyException" />
    public MissingApiKeyException(string service)
        : base($"Не найден API-ключ для сервиса: {service}") { }
}