using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Получение рыночных данных
/// </summary>
internal sealed class FetchMarketDataStep(IInvestApiFacade investApi, IDataStorageContext dataStorage)
    : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string Name => "Получение рыночных данных";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var weekAgo = now.AddDays(-7);

        foreach (var ticker in context.Tickers)
        {
            var marketdata = await investApi.Marketdata.GetHistoricalDataAsync(ticker, weekAgo, now);
            await dataStorage.Marketdata.TruncateAsync();
            await dataStorage.Marketdata.AddRangeAsync(marketdata);

            var fundamentalData = await investApi.Fundamentals.GetFundamentalsAsync(ticker);
            if (fundamentalData != null)
            {
                await dataStorage.Fundamentals.AddAsync(fundamentalData);
            }
        }
    }
}
