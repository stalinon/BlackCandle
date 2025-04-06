using BlackCandle.Application.Interfaces.Infrastructure;

using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BlackCandle.Telegram;

/// <summary>
///     Реализация Telegram-сервиса
/// </summary>
internal sealed class TelegramService(
    IOptions<TelegramOptions> options,
    ILoggerService logger) : ITelegramService
{
    private readonly ITelegramBotClient _bot = new TelegramBotClient(options.Value.BotToken);
    private readonly string _chatId = options.Value.ChatId;

    /// <inheritdoc />
    public async Task SendMessageAsync(string message, bool disableNotification = false)
    {
        try
        {
            await _bot.SendMessage(
                _chatId,
                message,
                ParseMode.Markdown,
                disableNotification: disableNotification);
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при отправке Telegram-сообщения", ex);
        }
    }

    /// <inheritdoc />
    public async Task SendFileAsync(Stream fileStream, string fileName, string caption = "")
    {
        try
        {
            var input = new InputFileStream(fileStream, fileName);

            await _bot.SendDocument(
                _chatId,
                input,
                caption,
                ParseMode.Markdown);
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при отправке файла в Telegram", ex);
        }
    }
}
