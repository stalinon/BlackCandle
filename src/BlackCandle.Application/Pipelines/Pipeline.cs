using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines;

/// <summary>
///     Пайплайн
/// </summary>
public abstract class Pipeline<TContext> where TContext : new()
{
    private readonly IEnumerable<IPipelineStep<TContext>> _steps;
    private readonly ILoggerService _logger;
    
    /// <summary>
    ///     Реакция на смену статуса шага
    /// </summary>
    event PipelineStepStatusChangedHandler<TContext>? OnStepStatusChanged;
    
    /// <summary>
    ///     Реакция на смену статуса
    /// </summary>
    public event PipelineStatusChangedHandler<TContext>? OnStatusChanged;

    /// <summary>
    ///     Статус пайплайна
    /// </summary>
    public PipelineStatus Status { get; private set; } = PipelineStatus.NotStarted;

    /// <inheritdoc cref="Pipeline{TContext}" />
    protected Pipeline(IEnumerable<IPipelineStep<TContext>> steps, ILoggerService logger)
    {
        _steps = steps;
        foreach (var pipelineStep in _steps)
        {
            pipelineStep.Status = PipelineStepStatus.NotStarted;
        }
        
        _logger = logger;
    }

    /// <summary>
    ///     Название пайплайна
    /// </summary>
    protected abstract string Name { get; }
    
    /// <summary>
    ///     Запуск пайплайна
    /// </summary>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.AddPrefix(Name);
        
        var context = new TContext();
        UpdatePipelineStatus(PipelineStatus.Running, context);

        foreach (var step in _steps)
        {
            try
            {
                UpdatePipelineStepStatus(PipelineStepStatus.Running, step, context);
                _logger.LogInfo($"Начинается шаг: {step.StepName}");
                await step.ExecuteAsync(context, cancellationToken);
                _logger.LogInfo($"Шаг завершен: {step.StepName}");
                UpdatePipelineStepStatus(PipelineStepStatus.Completed, step, context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка на шаге {step.StepName}", ex);
                UpdatePipelineStepStatus(PipelineStepStatus.Failed, step, context);
                UpdatePipelineStatus(PipelineStatus.Failed, context, ex);
                throw;
            }
        }
        
        UpdatePipelineStatus(PipelineStatus.Completed, context);
    }
    
    private void UpdatePipelineStepStatus(PipelineStepStatus newStatus, IPipelineStep<TContext> step, TContext context, Exception? ex = null)
    {
        step.Status = newStatus;
        OnStepStatusChanged?.Invoke(step.Status, step.StepName, context, ex);
    }
    
    private void UpdatePipelineStatus(PipelineStatus newStatus, TContext context, Exception? ex = null)
    {
        Status = newStatus;
        OnStatusChanged?.Invoke(newStatus, context, ex);
    }
}