using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Генерация сигналов
/// </summary>
internal sealed class GenerateSignalsStep(IInvestApiFacade investApi, IDataStorageContext dataStorage) : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string StepName => "Генерация сигналов";

    /// <inheritdoc />
    public override Task ExecuteAsync(PortfolioAnalysisContext context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}