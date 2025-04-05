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
    ///     Максимальная доля одного тикера в портфеле (%)
    /// </summary>
    public decimal MaxPositionPerTickerPercent { get; set; }

    /// <summary>
    ///     Минимальная сумма сделки
    /// </summary>
    public decimal MinTradeAmount { get; set; }

    /// <summary>
    ///     Расписание
    /// </summary>
    public CronSchedule Schedule { get; set; } = new();

    /// <summary>
    ///     Статус бота
    /// </summary>
    public BotStatus BotStatus { get; set; }
}
