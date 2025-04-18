using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Фундаментальный скоринг
/// </summary>
internal sealed class ScoreFundamentalsStep(IDataStorageContext dataStorage) : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string Name => "Фундаментальный скоринг";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        foreach (var ticker in context.Tickers)
        {
            var fundamental = await dataStorage.Fundamentals.GetByIdAsync(ticker.Symbol);

            if (fundamental is null)
            {
                continue;
            }

            var score = 0;

            if (fundamental.PERatio is < 15 and > 0)
            {
                score++;
            }

            if (fundamental.PBRatio is < 3 and > 0)
            {
                score++;
            }

            if (fundamental.DividendYield is > 4)
            {
                score++;
            }

            if (fundamental.ROE is > 10)
            {
                score++;
            }

            if (fundamental.MarketCap is > 100_000)
            {
                score++;
            }

            context.FundamentalScores[ticker] = score;
        }
    }
}
