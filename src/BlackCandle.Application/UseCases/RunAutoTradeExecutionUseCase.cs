using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Use-case для запуска автотрейдера
/// </summary>
public class RunAutoTradeExecutionUseCase : IUseCase<string>
{
    private readonly AutoTradeExecutionPipeline _pipeline = null!;
    private readonly IDataStorageContext _dataStorage = null!;

    /// <inheritdoc cref="RunAutoTradeExecutionUseCase"/>
    public RunAutoTradeExecutionUseCase(IPipelineFactory factory, IDataStorageContext dataStorage)
    {
        _pipeline = factory.Create<AutoTradeExecutionPipeline, AutoTradeExecutionContext>();
        _dataStorage = dataStorage;
    }

    /// <inheritdoc cref="RunAutoTradeExecutionUseCase"/>
    protected RunAutoTradeExecutionUseCase()
    {
    }

    /// <inheritdoc />
    public virtual async Task<OperationResult<string>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var tracker = new PipelineExecutionTracker<AutoTradeExecutionContext>();
        tracker.Attach(_pipeline, wasScheduled: false);

        await _pipeline.RunAsync(cancellationToken);

        await _dataStorage.PipelineRuns.AddAsync(tracker.GetRecord());

        return _pipeline.Status == PipelineStatus.Completed
            ? OperationResult<string>.Success("Торговля успешно закончена")
            : OperationResult<string>.Failure("Возникла ошибка при попытке провести торги");
    }
}
