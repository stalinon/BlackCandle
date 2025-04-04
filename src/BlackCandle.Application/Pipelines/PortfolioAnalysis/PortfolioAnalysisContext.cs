using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Контекст пайплайна `PortfolioAnalysis`
/// </summary>
public class PortfolioAnalysisContext
{
    /// <summary>
    ///     Время анализа
    /// </summary>
    public DateTime AnalysisTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Технические индикаторы
    /// </summary>
    public Dictionary<Ticker, List<TechnicalIndicator>> Indicators { get; set; } = new();
    
    /// <summary>
    ///     Скоры по фундаментальным данным
    /// </summary>
    public Dictionary<Ticker, int> FundamentalScores { get; set; } = new();
}