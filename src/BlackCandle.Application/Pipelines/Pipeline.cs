using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
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
    ///     Контекст пайплайна
    /// </summary>
    public TContext Context { get; } = new();
    
    /// <summary>
    ///     Реакция на смену статуса шага
    /// </summary>
    public event PipelineStepStatusChangedHandler<TContext>? OnStepStatusChanged;
    
    /// <summary>
    ///     Реакция на смену статуса
    /// </summary>
    public event PipelineStatusChangedHandler<TContext>? OnStatusChanged;

    /// <summary>
    ///     Статус пайплайна
    /// </summary>
    public PipelineStatus Status { get; private set; } = PipelineStatus.NotStarted;

    /// <summary>
    ///     Название пайплайна
    /// </summary>
    protected abstract string Name { get; }

    /// <inheritdoc cref="Pipeline{TContext}" />
    protected Pipeline(IEnumerable<IPipelineStep<TContext>> steps, ILoggerService logger)
    {
        _steps = steps;
        _logger = logger;
        
        // ReSharper disable once VirtualMemberCallInConstructor
        _logger.AddPrefix(Name);
        
        foreach (var pipelineStep in _steps)
        {
            pipelineStep.Status = PipelineStepStatus.NotStarted;
            pipelineStep.EarlyExitAction = EarlyExit;
            pipelineStep.Logger = _logger;
        }
    }
    
    /// <summary>
    ///     Запуск пайплайна
    /// </summary>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        UpdatePipelineStatus(PipelineStatus.Running, Context);

        foreach (var step in _steps)
        {
            try
            {
                UpdatePipelineStepStatus(PipelineStepStatus.Running, step, Context);
                _logger.LogInfo($"Начинается шаг: {step.StepName}");
                await step.ExecuteAsync(Context, cancellationToken);
                _logger.LogInfo($"Шаг завершен: {step.StepName}");

                if (Status != PipelineStatus.Running)
                {
                    return;
                }
                
                UpdatePipelineStepStatus(PipelineStepStatus.Completed, step, Context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка на шаге {step.StepName}", ex);
                UpdatePipelineStepStatus(PipelineStepStatus.Failed, step, Context);
                UpdatePipelineStatus(PipelineStatus.Failed, Context, ex);
                throw;
            }
        }
        
        UpdatePipelineStatus(PipelineStatus.Completed, Context);
    }

    /// <summary>
    ///     Ранний выход из пайплайна
    /// </summary>
    private void EarlyExit(TContext context, Exception exception, IPipelineStep<TContext> step)
    {
        UpdatePipelineStepStatus(PipelineStepStatus.Completed, step, Context);
        UpdatePipelineStatus(PipelineStatus.NotStarted, context, exception);
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