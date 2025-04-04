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
    ///     Торговые данные
    /// </summary>
    public Dictionary<Ticker, List<PriceHistoryPoint>> Marketdata { get; set; } = new();

    /// <summary>
    ///     Технические индикаторы
    /// </summary>
    public Dictionary<Ticker, Dictionary<string, decimal[]>> Indicators { get; set; } = new();
}