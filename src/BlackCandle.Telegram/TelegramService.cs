using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Helpers;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BlackCandle.Telegram;

/// <summary>
///     Реализация Telegram-сервиса
/// </summary>
internal class TelegramService(
    IBotSettingsService botSettingsService,
    ILoggerService logger) : ITelegramService
{
    /// <inheritdoc />
    public async Task SendMessageAsync(string message, bool disableNotification = false)
    {
        var (bot, chatId) = await GetBotSettings();

        try
        {
            await bot.SendMessage(
                chatId,
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
        var (bot, chatId) = await GetBotSettings();

        try
        {
            var input = new InputFileStream(fileStream, fileName);

            await bot.SendDocument(
                chatId,
                input,
                caption,
                ParseMode.Markdown);
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при отправке файла в Telegram", ex);
        }
    }

    /// <summary>
    ///     Получить бота
    /// </summary>
    protected virtual async Task<(ITelegramBotClient Bot, string ChatId)> GetBotSettings()
    {
        var botSettings = await botSettingsService.GetAsync();
        var telegramBotConfig = botSettings.ToTelegramConfig();
        return (new TelegramBotClient(telegramBotConfig.BotToken), telegramBotConfig.ChatId);
    }
}
