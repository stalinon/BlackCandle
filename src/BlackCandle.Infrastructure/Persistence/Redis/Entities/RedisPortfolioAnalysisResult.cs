using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Результат анализа портфеля для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisPortfolioAnalysisResult : IStorageEntity<PortfolioAnalysisResult>
{
    /// <summary>
    ///     Дата анализа
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    ///     Портфель на момент анализа
    /// </summary>
    public List<PortfolioAsset> Portfolio { get; set; } = new();

    /// <summary>
    ///     Сигналы
    /// </summary>
    public List<TradeSignal> Signals { get; set; } = new();

    /// <summary>
    ///     Использовались ли фундаментальные данные
    /// </summary>
    public bool FundamentalsUsed { get; set; }

    /// <summary>
    ///     Комментарий
    /// </summary>
    public string Commentary { get; set; } = string.Empty;

    /// <inheritdoc />
    public PortfolioAnalysisResult ToEntity() => new()
    {
        Date = Date,
        Portfolio = Portfolio,
        Signals = Signals,
        FundamentalsUsed = FundamentalsUsed,
        Commentary = Commentary,
    };

    /// <inheritdoc />
    public IStorageEntity<PortfolioAnalysisResult> ToStorageEntity(PortfolioAnalysisResult entity)
    {
        return new RedisPortfolioAnalysisResult
        {
            Date = entity.Date,
            Portfolio = entity.Portfolio,
            Signals = entity.Signals,
            FundamentalsUsed = entity.FundamentalsUsed,
            Commentary = entity.Commentary,
        };
    }
}
