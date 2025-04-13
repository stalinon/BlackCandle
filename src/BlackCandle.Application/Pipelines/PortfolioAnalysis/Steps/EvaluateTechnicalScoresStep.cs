using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Шаг оценки технических индикаторов
/// </summary>
internal sealed class EvaluateTechnicalScoresStep(
    IEnumerable<ISignalGenerationStrategy> strategies)
    : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc/>
    public override string Name => "Оценка тех. индикаторов по стратегиям";

    /// <inheritdoc/>
    public override Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        foreach (var ticker in context.Tickers)
        {
            if (!context.Indicators.TryGetValue(ticker, out var indicators))
            {
                continue;
            }

            var scores = new List<TechnicalScore>();
            foreach (var strategy in strategies)
            {
                var score = strategy.GenerateScore(ticker, indicators);
                if (score != null)
                {
                    scores.Add(score);
                }
            }

            context.TechnicalScores[ticker] = scores;
        }

        return Task.CompletedTask;
    }
}
