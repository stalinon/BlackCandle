using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Use-case для анализа портфеля
/// </summary>
internal sealed class RunPortfolioAnalysisUseCase(PortfolioAnalysisPipeline pipeline, IDataStorageContext dataStorage) : IUseCase<string>
{
    /// <inheritdoc />
    public async Task<OperationResult<string>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var tracker = new PipelineExecutionTracker<PortfolioAnalysisContext>();
        tracker.Attach(pipeline, wasScheduled: false);

        await pipeline.RunAsync(cancellationToken);

        await dataStorage.PipelineRuns.AddAsync(tracker.GetRecord());

        return pipeline.Status == PipelineStatus.Completed
            ? OperationResult<string>.Success("Портфель успешно проанализирован")
            : OperationResult<string>.Failure("Не удалось проанализировать портфель");
    }
}
