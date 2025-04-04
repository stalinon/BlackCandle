using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines;

/// <summary>
///     Абстрактная реализация шага пайплайна
/// </summary>
internal abstract class PipelineStep<TContext> : IPipelineStep<TContext>
    where TContext : new()
{
    /// <inheritdoc />
    public ILoggerService Logger { protected get; set; }

    /// <inheritdoc />
    public Action<TContext, Exception> EarlyExitAction { get; set; } = (ctx, ex) => { };

    /// <inheritdoc />
    public PipelineStepStatus Status { get; set; } = PipelineStepStatus.NotStarted;
    
    /// <inheritdoc />
    public abstract string StepName { get; }
    
    /// <inheritdoc />
    public abstract Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
}