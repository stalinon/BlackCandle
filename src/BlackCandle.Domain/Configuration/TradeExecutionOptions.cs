namespace BlackCandle.Domain.Configuration;

/// <summary>
///     Конфигурация исполнения заявок
/// </summary>
public sealed class TradeExecutionOptions
{
    /// <summary>
    ///     Максимальная цена сделки
    /// </summary>
    public decimal MaxTradeAmountRub { get; set; } = 10_000m;

    /// <summary>
    ///     Максимальное количество лотов в сделке
    /// </summary>
    public int MaxLotsPerTrade { get; set; } = 100;

    /// <summary>
    ///     Клонировать сущность
    /// </summary>
    public TradeExecutionOptions Copy() => (TradeExecutionOptions)MemberwiseClone();
}
