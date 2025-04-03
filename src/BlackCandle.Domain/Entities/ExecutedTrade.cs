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
    public string Ticker { get; set; } = string.Empty;

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
    public string Status { get; set; } = string.Empty;
}