using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Точка исторических данных (OHLCV)
/// </summary>
public class PriceHistoryPoint : IEntity
{
    /// <inheritdoc />
    public string Id => $"{Ticker}_{Date:yyyyMMddHHmmss}";

    /// <summary>
    ///     Тикер
    /// </summary>
    public Ticker Ticker { get; set; } = default!;

    /// <summary>
    ///     Дата
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    ///     Цена открытия
    /// </summary>
    public decimal Open { get; set; }

    /// <summary>
    ///     Максимум
    /// </summary>
    public decimal High { get; set; }

    /// <summary>
    ///     Минимум
    /// </summary>
    public decimal Low { get; set; }

    /// <summary>
    ///     Закрытие
    /// </summary>
    public decimal Close { get; set; }

    /// <summary>
    ///     Объем
    /// </summary>
    public long Volume { get; set; }
}
