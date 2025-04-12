namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Выбрасывается, когда бот не сконфигурирован. Отсутствуют настройки.
/// </summary>
public class BotNotConfiguredException : BlackCandleException
{
    /// <inheritdoc cref="BotNotConfiguredException"/>
    public BotNotConfiguredException(string message)
        : base(message)
    { }

    /// <inheritdoc cref="BotNotConfiguredException"/>
    public BotNotConfiguredException()
        : base("Отсутствует конфигурация бота")
    {
    }
}
