using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Use-case для запуска автотрейдера
/// </summary>
internal sealed class RunAutoTradeExecutionUseCase(AutoTradeExecutionPipeline pipeline, IDataStorageContext dataStorage) : IUseCase<string>
{
    /// <inheritdoc />
    public async Task<OperationResult<string>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var tracker = new PipelineExecutionTracker<AutoTradeExecutionContext>();
        tracker.Attach(pipeline, wasScheduled: false);

        await pipeline.RunAsync(cancellationToken);

        await dataStorage.PipelineRuns.AddAsync(tracker.GetRecord());

        return pipeline.Status == PipelineStatus.Completed
            ? OperationResult<string>.Success("Торговля успешно закончена")
            : OperationResult<string>.Failure("Возникла ошибка при попытке провести торги");
    }
}
