using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Исполненная сделка
/// </summary>
public class ExecutedTrade : IEntity
{
    /// <inheritdoc />
    public string Id => $"{Ticker}_{ExecutedAt:yyyyMMddHHmmss}";

    /// <summary>
    ///     Символ
    /// </summary>
    public Ticker Ticker { get; set; }

    /// <summary>
    ///     Сторона
    /// </summary>
    public TradeAction Side { get; set; }

    /// <summary>
    ///     Количество
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    ///     Цена исполнения
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Дата и время исполнения
    /// </summary>
    public DateTime ExecutedAt { get; set; }

    /// <summary>
    ///     Статус
    /// </summary>
    public TradeStatus Status { get; set; }
}