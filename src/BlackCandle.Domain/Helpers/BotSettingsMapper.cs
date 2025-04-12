using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;

namespace BlackCandle.Domain.Helpers;

/// <summary>
///     Маппер настроек
/// </summary>
public static class BotSettingsMapper
{
    /// <summary>
    ///     Конфигурация Tinkoff
    /// </summary>
    public static TinkoffClientOptions ToTinkoffConfig(this BotSettings settings)
    {
        if (settings.TinkoffClientOptions == null)
        {
            throw new BotNotConfiguredException("Клиент Tinkoff не сконфигурирован.");
        }

        return settings.TinkoffClientOptions;
    }

    /// <summary>
    ///     Конфигурация Telegram
    /// </summary>
    public static TelegramOptions ToTelegramConfig(this BotSettings settings)
    {
        if (settings.TelegramOptions == null)
        {
            throw new BotNotConfiguredException("Клиент Telegram не сконфигурирован.");
        }

        return settings.TelegramOptions;
    }
}
