using BlackCandle.Application.Interfaces.InvestApi;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Шаг подгрузки тикеров вне портфеля
/// </summary>
internal sealed class DiscoverNewTickersStep(
    IInvestApiFacade investApi)
    : PipelineStep<PortfolioAnalysisContext>
{
    private const int MaxTickers = 100;

    /// <inheritdoc/>
    public override string Name => "Получение бумаг вне портфеля";

    /// <inheritdoc/>
    public override async Task ExecuteAsync(PortfolioAnalysisContext context, CancellationToken cancellationToken = default)
    {
        var topTickers = await investApi.Instruments.GetTopTickersAsync(MaxTickers);
        context.Tickers.AddRange(topTickers);
    }
}
