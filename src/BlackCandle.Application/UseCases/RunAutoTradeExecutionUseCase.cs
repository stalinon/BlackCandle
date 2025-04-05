using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Use-case для запуска автотрейдера
/// </summary>
internal sealed class RunAutoTradeExecutionUseCase(AutoTradeExecutionPipeline pipeline) : IUseCase<string>
{
    /// <inheritdoc />
    public async Task<OperationResult<string>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await pipeline.RunAsync(cancellationToken);

        return pipeline.Status == PipelineStatus.Completed
            ? OperationResult<string>.Success("Торговля успешно закончена")
            : OperationResult<string>.Failure("Возникла ошибка при попытке провести торги");
    }
}
