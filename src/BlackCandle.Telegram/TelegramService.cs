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
    private ITelegramBotClient? _bot;
    private string? _chatId;

    /// <inheritdoc />
    public async Task SendMessageAsync(string message, bool disableNotification = false)
    {
        var (bot, chatId) = await GetBotSettings();

        try
        {
            if (message.Length > 4000)
            {
                // Слишком длинное сообщение — отправляем как файл
                using var stream = new MemoryStream();
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(message);
                await writer.FlushAsync();
                stream.Position = 0;

                await SendFileAsync(
                    stream,
                    $"report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.md",
                    "Отчёт слишком большой, отправлен файлом.");
            }
            else
            {
                // Нормальная длина — шлём как текст
                await bot.SendMessage(
                    chatId,
                    message,
                    ParseMode.Markdown,
                    disableNotification: disableNotification);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при отправке Telegram-сообщения", ex);
        }
    }

    /// <inheritdoc />
    public virtual async Task SendFileAsync(Stream fileStream, string fileName, string caption = "")
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
        if (_bot != null && !string.IsNullOrEmpty(_chatId))
        {
            return (_bot, _chatId);
        }

        var botSettings = await botSettingsService.GetAsync();
        var telegramBotConfig = botSettings.ToTelegramConfig();
        _bot = new TelegramBotClient(telegramBotConfig.BotToken);
        _chatId = telegramBotConfig.ChatId;

        return (_bot, _chatId);
    }
}
