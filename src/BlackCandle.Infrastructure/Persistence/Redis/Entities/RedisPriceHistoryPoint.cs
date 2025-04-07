using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Точка исторических данных (OHLCV) для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisPriceHistoryPoint : IStorageEntity<PriceHistoryPoint>
{
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

    /// <inheritdoc />
    public PriceHistoryPoint ToEntity() => new()
    {
        Ticker = Ticker,
        Date = Date,
        Open = Open,
        High = High,
        Low = Low,
        Close = Close,
        Volume = Volume,
    };

    /// <inheritdoc />
    public IStorageEntity<PriceHistoryPoint> ToStorageEntity(PriceHistoryPoint entity)
    {
        return new RedisPriceHistoryPoint
        {
            Ticker = entity.Ticker,
            Date = entity.Date,
            Open = entity.Open,
            High = entity.High,
            Low = entity.Low,
            Close = entity.Close,
            Volume = entity.Volume,
        };
    }
}
