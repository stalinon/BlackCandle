namespace BlackCandle.Infrastructure.Trading;

/// <summary>
///     Конфигурация исполнения заявок
/// </summary>
internal sealed class TradeExecutionOptions
{
    /// <summary>
    ///     Максимальная цена сделки
    /// </summary>
    public decimal MaxTradeAmountRub { get; set; } = 10_000m;

    /// <summary>
    ///     Максимальное количество лотов в сделке
    /// </summary>
    public int MaxLotsPerTrade { get; set; } = 100;
}
