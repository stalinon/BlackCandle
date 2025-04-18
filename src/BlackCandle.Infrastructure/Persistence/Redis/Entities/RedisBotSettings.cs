using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Сущность настроек бота для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisBotSettings : IStorageEntity<BotSettings>
{
    /// <summary>
    ///     Разрешено ли автоисполнение
    /// </summary>
    public bool EnableAutoTrading { get; set; }

    /// <summary>
    ///     Настройки Telegram
    /// </summary>
    public TelegramOptions? TelegramOptions { get; set; }

    /// <summary>
    ///     Настройки клиента Tinkoff
    /// </summary>
    public TinkoffClientOptions? TinkoffClientOptions { get; set; }

    /// <summary>
    ///     Лимиты сделок
    /// </summary>
    public TradeLimitOptions TradeLimit { get; set; } = new();

    /// <summary>
    ///    Исполнение заявок
    /// </summary>
    public TradeExecutionOptions TradeExecution { get; set; } = new();

    /// <summary>
    ///     Расписание
    /// </summary>
    public Dictionary<string, CronSchedule> Schedules { get; set; } = new();

    /// <summary>
    ///     Статус бота
    /// </summary>
    public BotStatus BotStatus { get; set; }

    /// <inheritdoc/>
    public BotSettings ToEntity() => new()
    {
        EnableAutoTrading = EnableAutoTrading,
        TelegramOptions = TelegramOptions?.Copy(),
        TinkoffClientOptions = TinkoffClientOptions?.Copy(),
        TradeLimit = TradeLimit.Copy(),
        TradeExecution = TradeExecution.Copy(),
        Schedules = Schedules.ToDictionary(s => s.Key, s => s.Value.Copy()),
        BotStatus = BotStatus,
    };

    /// <inheritdoc/>
    public IStorageEntity<BotSettings> ToStorageEntity(BotSettings entity)
    {
        return new RedisBotSettings
        {
            EnableAutoTrading = entity.EnableAutoTrading,
            TelegramOptions = entity.TelegramOptions?.Copy(),
            TinkoffClientOptions = entity.TinkoffClientOptions?.Copy(),
            TradeLimit = entity.TradeLimit.Copy(),
            TradeExecution = entity.TradeExecution.Copy(),
            Schedules = entity.Schedules.ToDictionary(s => s.Key, s => s.Value.Copy()),
            BotStatus = entity.BotStatus,
        };
    }
}
