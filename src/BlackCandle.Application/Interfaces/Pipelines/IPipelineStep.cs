using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Interfaces.Pipelines;

/// <summary>
///     Шаг пайплайна
/// </summary>
public interface IPipelineStep<TContext>
{
    /// <summary>
    ///     Статус
    /// </summary>
    PipelineStepStatus Status { get; set; }
    
    /// <summary>
    ///     Название шага
    /// </summary>
    string StepName { get; }
    
    /// <summary>
    ///     Выполнить
    /// </summary>
    Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
}