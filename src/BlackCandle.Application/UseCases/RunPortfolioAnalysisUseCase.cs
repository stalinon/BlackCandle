using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Use-case для анализа портфеля
/// </summary>
public class RunPortfolioAnalysisUseCase : IUseCase<string>
{
    private readonly PortfolioAnalysisPipeline _pipeline = null!;
    private readonly IDataStorageContext _dataStorage = null!;

    /// <inheritdoc cref="RunPortfolioAnalysisUseCase" />
    public RunPortfolioAnalysisUseCase(IPipelineFactory factory, IDataStorageContext dataStorage)
    {
        _pipeline = factory.Create<PortfolioAnalysisPipeline, PortfolioAnalysisContext>();
        _dataStorage = dataStorage;
    }

    /// <inheritdoc cref="RunPortfolioAnalysisUseCase" />
    protected RunPortfolioAnalysisUseCase()
    {
    }

    /// <inheritdoc />
    public virtual async Task<OperationResult<string>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var tracker = new PipelineExecutionTracker<PortfolioAnalysisContext>();
        tracker.Attach(_pipeline, wasScheduled: false);

        await _pipeline.RunAsync(cancellationToken);

        await _dataStorage.PipelineRuns.AddAsync(tracker.GetRecord());

        return _pipeline.Status == PipelineStatus.Completed
            ? OperationResult<string>.Success("Портфель успешно проанализирован")
            : OperationResult<string>.Failure("Не удалось проанализировать портфель");
    }
}
