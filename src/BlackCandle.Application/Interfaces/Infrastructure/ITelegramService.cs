namespace BlackCandle.Application.Interfaces.Infrastructure;

/// <summary>
///     Сервис для взаимодействия с Telegram
/// </summary>
public interface ITelegramService
{
    /// <summary>
    ///     Отправить сообщение
    /// </summary>
    Task SendMessageAsync(string message, bool disableNotification = false);

    /// <summary>
    ///     Отправить файл
    /// </summary>
    Task SendFileAsync(Stream fileStream, string fileName, string caption = "");
}
