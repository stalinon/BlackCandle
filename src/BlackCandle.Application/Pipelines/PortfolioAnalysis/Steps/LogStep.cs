using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Pipelines;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Логирование
/// </summary>
internal sealed class LogStep(IInvestApiFacade investApi, IDataStorageContext dataStorage) : IPipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public string StepName => "Логирование";

    /// <inheritdoc />
    public Task ExecuteAsync(PortfolioAnalysisContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}