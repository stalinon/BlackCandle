using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Контекст пайплайна `PortfolioAnalysis`
/// </summary>
public class PortfolioAnalysisContext
{
    /// <summary>
    ///     Тикеры, участвующие в анализе
    /// </summary>
    public List<Ticker> Tickers { get; set; } = new();

    /// <summary>
    ///     Время анализа
    /// </summary>
    public DateTime AnalysisTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Технические индикаторы
    /// </summary>
    public Dictionary<Ticker, List<TechnicalIndicator>> Indicators { get; set; } = [];

    /// <summary>
    ///     Скоры по фундаментальным данным
    /// </summary>
    public Dictionary<Ticker, int> FundamentalScores { get; set; } = [];
}
