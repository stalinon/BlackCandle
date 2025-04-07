using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Выполненная сделка для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisExecutedTrade : IStorageEntity<ExecutedTrade>
{
    /// <summary>
    ///     Символ
    /// </summary>
    public Ticker Ticker { get; set; } = default!;

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

    /// <inheritdoc />
    public ExecutedTrade ToEntity() => new()
    {
        Ticker = Ticker,
        Side = Side,
        Quantity = Quantity,
        Price = Price,
        ExecutedAt = ExecutedAt,
        Status = Status,
    };

    /// <inheritdoc />
    public IStorageEntity<ExecutedTrade> ToStorageEntity(ExecutedTrade entity)
    {
        return new RedisExecutedTrade
        {
            Ticker = entity.Ticker,
            Side = entity.Side,
            Quantity = entity.Quantity,
            Price = entity.Price,
            ExecutedAt = entity.ExecutedAt,
            Status = entity.Status,
        };
    }
}
