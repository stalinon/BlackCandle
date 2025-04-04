using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Pipelines;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Генерация сигналов
/// </summary>
internal sealed class GenerateSignalsStep(IInvestApiFacade investApi, IDataStorageContext dataStorage) : IPipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public string StepName => "Генерация сигналов";

    /// <inheritdoc />
    public Task ExecuteAsync(PortfolioAnalysisContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}