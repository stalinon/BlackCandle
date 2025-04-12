using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Настройки бота
/// </summary>
public class BotSettings : IEntity
{
    /// <summary>
    ///     Идентификатор по умолчанию
    /// </summary>
    public const string DefaultId = "GlobalSettings";

    /// <inheritdoc />
    public string Id => DefaultId;

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

    /// <summary>
    ///     Клонировать сущность
    /// </summary>
    public BotSettings Copy()
    {
        return new()
        {
            EnableAutoTrading = EnableAutoTrading,
            TelegramOptions = TelegramOptions?.Copy(),
            TinkoffClientOptions = TinkoffClientOptions?.Copy(),
            TradeLimit = TradeLimit.Copy(),
            TradeExecution = TradeExecution.Copy(),
            Schedules = Schedules.ToDictionary(s => s.Key, s => s.Value.Copy()),
            BotStatus = BotStatus,
        };
    }
}
