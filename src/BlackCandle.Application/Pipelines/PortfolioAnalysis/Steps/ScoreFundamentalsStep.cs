using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Pipelines;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Фундаментальный скоринг
/// </summary>
internal sealed class ScoreFundamentalsStep(IInvestApiFacade investApi, IDataStorageContext dataStorage) : IPipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public string StepName => "Фундаментальный скоринг";

    /// <inheritdoc />
    public Task ExecuteAsync(PortfolioAnalysisContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}