using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Результат расчета индикатора для Redis
/// </summary>
internal sealed class RedisIndicatorResult : IStorageEntity<IndicatorResult>
{
    /// <summary>
    ///     Символ тикера
    /// </summary>
    public Ticker Ticker { get; set; } = default!;

    /// <summary>
    ///     Тип индикатора
    /// </summary>
    public IndicatorType IndicatorType { get; set; }

    /// <summary>
    ///     Дата значения
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    ///     Значение
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    ///     Дополнительные данные (если есть)
    /// </summary>
    public Dictionary<string, decimal> ExtraData { get; set; } = [];

    /// <inheritdoc/>
    public IndicatorResult ToEntity() => new()
    {
        Ticker = Ticker,
        IndicatorType = IndicatorType,
        Date = Date,
        Value = Value,
        ExtraData = ExtraData,
    };

    /// <inheritdoc/>
    public IStorageEntity<IndicatorResult> ToStorageEntity(IndicatorResult entity)
    {
        return new RedisIndicatorResult
        {
            Ticker = entity.Ticker,
            IndicatorType = entity.IndicatorType,
            Date = entity.Date,
            Value = entity.Value,
            ExtraData = entity.ExtraData,
        };
    }
}
