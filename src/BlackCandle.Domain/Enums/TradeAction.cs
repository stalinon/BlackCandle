namespace BlackCandle.Domain.Enums;

/// <summary>
///     Торговое действие: покупка, продажа или удержание
/// </summary>
public enum TradeAction
{
    /// <summary>
    ///     Покупать
    /// </summary>
    Buy,
    /// <summary>
    ///     Продавать
    /// </summary>
    Sell,
    /// <summary>
    ///     Удерживать
    /// </summary>
    Hold
}