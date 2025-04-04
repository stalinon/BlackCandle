using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Фундаментальный скоринг
/// </summary>
internal sealed class ScoreFundamentalsStep(IInvestApiFacade investApi, IDataStorageContext dataStorage) : IPipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public PipelineStepStatus Status { get; set; }

    /// <inheritdoc />
    public string StepName => "Фундаментальный скоринг";

    /// <inheritdoc />
    public async Task ExecuteAsync(PortfolioAnalysisContext context, CancellationToken cancellationToken = default)
    {
        var portfolio = await dataStorage.PortfolioAssets.GetAllAsync();
        foreach (var asset in portfolio)
        {
            var ticker = asset.Ticker;
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

            if (fundamental.MarketCap is > 100_000) // в млн руб
            {
                score++;
            }

            context.FundamentalScores[ticker] = score;
        }
    }
}