using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Генерация сигналов на основе тех. индикаторов и фундаментала
/// </summary>
internal sealed class GenerateSignalsStep(IDataStorageContext dataStorage) : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string Name => "Генерация сигналов";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var ticker in context.Tickers)
        {
            var score = context.FundamentalScores.GetValueOrDefault(ticker, 0);
            if (score == default)
            {
                continue;
            }

            var techScores = context.TechnicalScores.GetValueOrDefault(ticker, new());
            if (techScores.Count == 0)
            {
                continue;
            }

            var signal = new TradeSignal
            {
                Ticker = ticker,
                Date = now,
                FundamentalScore = score,
                TechnicalScores = techScores,
                Action = ResolveAction(score, techScores),
                Confidence = ResolveConfidence(score, techScores),
                Reason = $"Фундаментальный скор: {score}, тех.индикаторы: {string.Join(", ", techScores.Select(s => $"{s.IndicatorName}:{s.Score}"))}",
            };

            await dataStorage.TradeSignals.AddAsync(signal);
        }
    }

    private static TradeAction ResolveAction(int fScore, List<TechnicalScore> tScores)
    {
        var total = fScore + tScores.Sum(t => t.Score);
        return total switch
        {
            >= 5 => TradeAction.Buy,
            <= 1 => TradeAction.Sell,
            _ => TradeAction.Hold,
        };
    }

    private static ConfidenceLevel ResolveConfidence(int fScore, List<TechnicalScore> tScores)
    {
        var total = fScore + tScores.Sum(t => t.Score);
        return total switch
        {
            >= 6 => ConfidenceLevel.High,
            >= 3 => ConfidenceLevel.Medium,
            _ => ConfidenceLevel.Low,
        };
    }
}
