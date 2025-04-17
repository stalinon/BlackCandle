using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;

using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Пайплайн автоматического исполнения сделок
/// </summary>
public class AutoTradeExecutionPipeline : Pipeline<AutoTradeExecutionContext>
{
    /// <inheritdoc cref="AutoTradeExecutionPipeline" />
    public AutoTradeExecutionPipeline(
        IServiceScope scope,
        ILoggerService logger)
        : base(scope, logger)
    { }

    /// <inheritdoc cref="AutoTradeExecutionPipeline" />
    protected AutoTradeExecutionPipeline()
    { }

    /// <inheritdoc />
    public override string Name => "Автотрейдинг";
}
