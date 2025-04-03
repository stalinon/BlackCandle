using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Инструмент в портфеле
/// </summary>
public class PortfolioAsset : IEntity
{
    /// <inheritdoc />
    public string Id => $"{Ticker.Symbol}_{PurchaseDate:yyyyMMdd}";

    /// <summary>
    ///     Тикер бумаги
    /// </summary>
    public Ticker Ticker { get; set; } = new();

    /// <summary>
    ///     Количество
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    ///     Цена покупки
    /// </summary>
    public decimal PurchasePrice { get; set; }

    /// <summary>
    ///     Дата покупки
    /// </summary>
    public DateTime PurchaseDate { get; set; }
}