using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Use-case для предпросмотра автотрейда без реального исполнения
/// </summary>
internal sealed class PreviewTradeExecutionUseCase(AutoTradeExecutionPipeline pipeline)
    : IUseCase<IReadOnlyCollection<OrderPreview>>
{
    /// <inheritdoc />
    public async Task<OperationResult<IReadOnlyCollection<OrderPreview>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        pipeline.Context.PreviewMode = true;
        await pipeline.RunAsync(cancellationToken);

        return pipeline.Status == PipelineStatus.Completed
            ? OperationResult<IReadOnlyCollection<OrderPreview>>.Success(pipeline.Context.PreviewOrders)
            : OperationResult<IReadOnlyCollection<OrderPreview>>.Failure("Не удалось сформировать предпросмотр торговли");
    }
}
