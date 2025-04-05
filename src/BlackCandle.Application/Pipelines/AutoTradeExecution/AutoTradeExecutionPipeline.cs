using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Пайплайн автоматического исполнения сделок
/// </summary>
public class AutoTradeExecutionPipeline : Pipeline<AutoTradeExecutionContext>
{
    /// <inheritdoc cref="AutoTradeExecutionPipeline" />
    public AutoTradeExecutionPipeline(
        IEnumerable<IPipelineStep<AutoTradeExecutionContext>> steps,
        ILoggerService logger)
        : base(steps, logger)
    { }

    /// <inheritdoc cref="AutoTradeExecutionPipeline" />
    public AutoTradeExecutionPipeline()
    { }

    /// <inheritdoc />
    public override string Name => "Автотрейдинг";
}
