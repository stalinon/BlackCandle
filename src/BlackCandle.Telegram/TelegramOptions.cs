namespace BlackCandle.Telegram;

/// <summary>
///     Настройки Telegram-бота
/// </summary>
public class TelegramOptions
{
    /// <summary>
    ///     Токен бота
    /// </summary>
    public string BotToken { get; set; } = string.Empty;
    
    /// <summary>
    ///     Идентификатор чата
    /// </summary>
    public string ChatId { get; set; } = string.Empty;
}