namespace BlackCandle.Domain.Configuration;

/// <summary>
///     Настройки ограничений
/// </summary>
public sealed class TradeLimitOptions
{
    /// <summary>
    ///     Минимальная цена сделки
    /// </summary>
    public decimal MinTradeAmountRub { get; set; } = 1000m;

    /// <summary>
    ///     Максимальная позиция
    /// </summary>
    public decimal MaxPositionSharePercent { get; set; } = 25m;

    /// <summary>
    ///     Максимальная позиция по секторам
    /// </summary>
    public int? MaxSectorPositions { get; set; }

    /// <summary>
    ///     Клонировать сущность
    /// </summary>
    public TradeLimitOptions Copy() => (TradeLimitOptions)MemberwiseClone();
}
