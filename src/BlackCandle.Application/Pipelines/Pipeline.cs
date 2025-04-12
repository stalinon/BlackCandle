using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines;

/// <summary>
///     Пайплайн
/// </summary>
public abstract class Pipeline<TContext>
    where TContext : new()
{
    private readonly List<IPipelineStep<TContext>> _steps = new();
    private readonly ILoggerService _logger = null!;

    /// <inheritdoc cref="Pipeline{TContext}" />
    protected Pipeline()
    { }

    /// <inheritdoc cref="Pipeline{TContext}" />
    protected Pipeline(IEnumerable<IPipelineStep<TContext>> steps, ILoggerService logger)
    {
        _steps = steps.ToList();
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
    ///     Реакция на смену статуса шага
    /// </summary>
    public event PipelineStepStatusChangedHandler<TContext>? OnStepStatusChanged;

    /// <summary>
    ///     Реакция на смену статуса
    /// </summary>
    public event PipelineStatusChangedHandler<TContext>? OnStatusChanged;

    /// <summary>
    ///     Контекст пайплайна
    /// </summary>
    public virtual TContext Context { get; internal set; } = new();

    /// <summary>
    ///     Статус пайплайна
    /// </summary>
    public virtual PipelineStatus Status { get; private set; } = PipelineStatus.NotStarted;

    /// <summary>
    ///     Название пайплайна
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    ///     Запуск пайплайна
    /// </summary>
    public virtual async Task RunAsync(CancellationToken cancellationToken = default)
    {
        UpdatePipelineStatus(PipelineStatus.Running, Context);
        _logger.LogInfo($"Запущен пайплайн: {Name}");

        foreach (var step in _steps)
        {
            try
            {
                UpdatePipelineStepStatus(PipelineStepStatus.Running, step, Context);
                _logger.LogInfo($"Начинается шаг: {step.Name}");
                await step.ExecuteAsync(Context, cancellationToken);
                _logger.LogInfo($"Шаг завершен: {step.Name}");

                if (Status != PipelineStatus.Running)
                {
                    return;
                }

                UpdatePipelineStepStatus(PipelineStepStatus.Completed, step, Context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка на шаге {step.Name}", ex);
                UpdatePipelineStepStatus(PipelineStepStatus.Failed, step, Context, ex);
                UpdatePipelineStatus(PipelineStatus.Failed, Context, ex);
                _logger.LogInfo($"Необработанная ошибка в пайплайне: {Name}");
                throw;
            }
        }

        UpdatePipelineStatus(PipelineStatus.Completed, Context);
        _logger.LogInfo($"Завершен пайплайн: {Name}");
    }

    /// <summary>
    ///     Добавить шаг в пайплайн
    /// </summary>
    internal void AddStep(IPipelineStep<TContext> step)
    {
        _steps.Add(step);
    }

    private void UpdatePipelineStepStatus(PipelineStepStatus newStatus, IPipelineStep<TContext> step, TContext context, Exception? ex = null)
    {
        step.Status = newStatus;
        OnStepStatusChanged?.Invoke(step.Status, step.Name, context, ex);
    }

    private void UpdatePipelineStatus(PipelineStatus newStatus, TContext context, Exception? ex = null)
    {
        Status = newStatus;
        OnStatusChanged?.Invoke(newStatus, context, ex);
    }

    /// <summary>
    ///     Ранний выход из пайплайна
    /// </summary>
    private void EarlyExit(TContext context, Exception exception, IPipelineStep<TContext> step)
    {
        UpdatePipelineStepStatus(PipelineStepStatus.Completed, step, Context);
        UpdatePipelineStatus(PipelineStatus.NotStarted, context, exception);
    }
}
