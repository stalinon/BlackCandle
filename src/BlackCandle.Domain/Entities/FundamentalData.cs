using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Фундаментальные метрики бумаги
/// </summary>
public class FundamentalData : IEntity
{
    /// <inheritdoc />
    public string Id => Ticker;

    /// <summary>
    ///     Символ тикера
    /// </summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>
    ///     P/E
    /// </summary>
    public decimal? PERatio { get; set; }

    /// <summary>
    ///     P/B
    /// </summary>
    public decimal? PBRatio { get; set; }

    /// <summary>
    ///     Дивдоходность
    /// </summary>
    public decimal? DividendYield { get; set; }

    /// <summary>
    ///     Капитализация
    /// </summary>
    public decimal? MarketCap { get; set; }

    /// <summary>
    ///     Рентабельность капитала
    /// </summary>
    public decimal? ROE { get; set; }

    /// <summary>
    ///     Последнее обновление
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
