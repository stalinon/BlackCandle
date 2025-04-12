using BlackCandle.Domain.Attributes;

namespace BlackCandle.Domain.Configuration;

/// <summary>
///     Настройки Telegram-бота
/// </summary>
public class TelegramOptions
{
    /// <summary>
    ///     Токен бота
    /// </summary>
    [Secret]
    public string BotToken { get; set; } = string.Empty;

    /// <summary>
    ///     Идентификатор чата
    /// </summary>
    public string ChatId { get; set; } = string.Empty;

    /// <summary>
    ///     Клонировать сущность
    /// </summary>
    public TelegramOptions Copy() => (TelegramOptions)MemberwiseClone();
}
