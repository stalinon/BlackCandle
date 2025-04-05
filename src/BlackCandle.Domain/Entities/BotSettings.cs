using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Настройки бота
/// </summary>
public class BotSettings : IEntity
{
    /// <inheritdoc />
    public string Id => "GlobalSettings";

    /// <summary>
    ///     Разрешено ли автоисполнение
    /// </summary>
    public bool EnableAutoTrading { get; set; }

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
}
