using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Получение рыночных данных
/// </summary>
internal sealed class FetchMarketDataStep(IInvestApiFacade investApi, IDataStorageContext dataStorage) : IPipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public PipelineStepStatus Status { get; set; }
    
    /// <inheritdoc />
    public string StepName => "Получение рыночных данных";

    /// <inheritdoc />
    public async Task ExecuteAsync(PortfolioAnalysisContext context, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var weekAgo = now.AddDays(-7);
        
        var portfolio = await dataStorage.PortfolioAssets.GetAllAsync();
        foreach (var asset in portfolio)
        {
            var marketdata = await investApi.Marketdata.GetHistoricalDataAsync(asset.Ticker, weekAgo, now);
            await dataStorage.Marketdata.TruncateAsync();
            await dataStorage.Marketdata.AddRangeAsync(marketdata);

            var fundamentalData = await investApi.Fundamentals.GetFundamentalsAsync(asset.Ticker);
            if (fundamentalData != null)
            {
                await dataStorage.Fundamentals.AddAsync(fundamentalData);
            }
        }
    }
}