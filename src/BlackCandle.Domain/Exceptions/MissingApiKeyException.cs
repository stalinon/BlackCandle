namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Выбрасывается, если не найден API-ключ.
/// </summary>
/// <inheritdoc cref="MissingApiKeyException" />
public class MissingApiKeyException(string service) : BlackCandleException($"Не найден API-ключ для сервиса: {service}")
{
}
