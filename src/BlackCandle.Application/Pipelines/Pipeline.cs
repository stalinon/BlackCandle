using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Pipelines;

namespace BlackCandle.Application.Pipelines;

/// <summary>
///     Пайплайн
/// </summary>
public abstract class Pipeline<TContext>(IEnumerable<IPipelineStep<TContext>> steps, ILoggerService logger)
    where TContext : new()
{
    /// <summary>
    ///     Название пайплайна
    /// </summary>
    protected abstract string Name { get; }
    
    /// <summary>
    ///     Запуск пайплайна
    /// </summary>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var context = new TContext();

        foreach (var step in steps)
        {
            try
            {
                logger.LogInfo($"[{Name}] Начинается шаг: {step.StepName}");
                await step.ExecuteAsync(context, cancellationToken);
                logger.LogInfo($"[{Name}] Шаг завершен: {step.StepName}");
            }
            catch (Exception ex)
            {
                logger.LogError($"[{Name}] Ошибка на шаге {step.StepName}", ex);
                throw;
            }
        }
    }
}