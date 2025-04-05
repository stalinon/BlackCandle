using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Результат анализа портфеля
/// </summary>
public class PortfolioAnalysisResult : IEntity
{
    /// <inheritdoc />
    public string Id => Date.ToString("yyyyMMdd");

    /// <summary>
    ///     Дата анализа
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    ///     Портфель на момент анализа
    /// </summary>
    public List<PortfolioAsset> Portfolio { get; set; } = [];

    /// <summary>
    ///     Сигналы
    /// </summary>
    public List<TradeSignal> Signals { get; set; } = [];

    /// <summary>
    ///     Использовались ли фундаментальные данные
    /// </summary>
    public bool FundamentalsUsed { get; set; }

    /// <summary>
    ///     Комментарий
    /// </summary>
    public string Commentary { get; set; } = string.Empty;
}
