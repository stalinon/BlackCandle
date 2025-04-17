using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Use-case для предпросмотра автотрейда без реального исполнения
/// </summary>
public class PreviewTradeExecutionUseCase : IUseCase<IReadOnlyCollection<OrderPreview>>
{
    private readonly AutoTradeExecutionPipeline _pipeline = null!;

    /// <inheritdoc cref="PreviewTradeExecutionUseCase" />
    public PreviewTradeExecutionUseCase(IPipelineFactory factory)
    {
        _pipeline = factory.Create<AutoTradeExecutionPipeline, AutoTradeExecutionContext>();
    }

    /// <inheritdoc cref="PreviewTradeExecutionUseCase" />
    protected PreviewTradeExecutionUseCase()
    {
    }

    /// <inheritdoc />
    public virtual async Task<OperationResult<IReadOnlyCollection<OrderPreview>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _pipeline.Context.PreviewMode = true;
        await _pipeline.RunAsync(cancellationToken);

        return _pipeline.Status == PipelineStatus.Completed
            ? OperationResult<IReadOnlyCollection<OrderPreview>>.Success(_pipeline.Context.PreviewOrders)
            : OperationResult<IReadOnlyCollection<OrderPreview>>.Failure("Не удалось сформировать предпросмотр торговли");
    }
}
