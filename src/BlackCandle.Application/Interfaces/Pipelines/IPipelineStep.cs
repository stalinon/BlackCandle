namespace BlackCandle.Application.Interfaces.Pipelines;

/// <summary>
///     Шаг пайплайна
/// </summary>
public interface IPipelineStep<TContext>
{
    /// <summary>
    ///     Название шага
    /// </summary>
    string StepName { get; }
    
    /// <summary>
    ///     Выполнить
    /// </summary>
    Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
}