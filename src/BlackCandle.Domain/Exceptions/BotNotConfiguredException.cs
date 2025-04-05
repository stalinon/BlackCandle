namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Выбрасывается, когда бот не сконфигурирован. Отсутствуют настройки.
/// </summary>
public class BotNotConfiguredException()
    : BlackCandleException("Отсутствует конфигурация бота");
