using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Инструмент в портфеле
/// </summary>
public class PortfolioAsset : IEntity
{
    /// <inheritdoc />
    public string Id => $"{Ticker.Symbol}";

    /// <summary>
    ///     Тикер бумаги
    /// </summary>
    public Ticker Ticker { get; set; } = new();

    /// <summary>
    ///     Количество
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    ///     Цена
    /// </summary>
    public decimal CurrentValue { get; set; }
}