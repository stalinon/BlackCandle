using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Pipelines;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Расчет технических индикаторов
/// </summary>
internal sealed class CalculateIndicatorsStep(IInvestApiFacade investApi, IDataStorageContext dataStorage) : IPipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public string StepName => "Расчет тех. индикаторов";

    /// <inheritdoc />
    public Task ExecuteAsync(PortfolioAnalysisContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}