using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Результат технического анализа
/// </summary>
public class IndicatorResult : IEntity
{
    /// <inheritdoc />
    public string Id => $"{Ticker}_{IndicatorType}_{Date:yyyyMMdd}";

    /// <summary>
    ///     Символ тикера
    /// </summary>
    public Ticker Ticker { get; set; }

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
    public Dictionary<string, decimal> ExtraData { get; set; } = new();
}