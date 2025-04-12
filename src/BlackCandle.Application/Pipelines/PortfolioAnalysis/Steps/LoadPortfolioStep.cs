using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Шаг загрузки портфолио
/// </summary>
internal sealed class LoadPortfolioStep(IInvestApiFacade investApi, IDataStorageContext dataStorage)
    : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string Name => "Загрузка портфолио";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var portfolio = await investApi.Portfolio.GetPortfolioAsync();
        await dataStorage.PortfolioAssets.TruncateAsync();
        await dataStorage.PortfolioAssets.AddRangeAsync(portfolio);

        context.Tickers.AddRange(portfolio.Select(a => a.Ticker));
    }
}
