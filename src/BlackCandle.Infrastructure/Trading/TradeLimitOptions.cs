namespace BlackCandle.Infrastructure.Trading;

/// <summary>
///     Настройки ограничений
/// </summary>
internal sealed class TradeLimitOptions
{
    /// <summary>
    ///     Минимальная цена сделки
    /// </summary>
    public decimal MinTradeAmountRub { get; set; } = 1000m;

    /// <summary>
    ///     Максимальная позиция
    /// </summary>
    public decimal MaxPositionSharePercent { get; set; } = 25m;
}
