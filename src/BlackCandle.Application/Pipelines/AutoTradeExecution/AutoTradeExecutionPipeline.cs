using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Пайплайн автоматического исполнения сделок
/// </summary>
public sealed class AutoTradeExecutionPipeline(
    IEnumerable<IPipelineStep<AutoTradeExecutionContext>> steps,
    ILoggerService logger)
    : Pipeline<AutoTradeExecutionContext>(steps, logger)
{
    /// <inheritdoc />
    protected override string Name => "Автотрейдинг";
}