using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines;

/// <summary>
///     Трекер пайплайнов
/// </summary>
internal class PipelineExecutionTracker<TContext>
    where TContext : new()
{
    private readonly PipelineExecutionRecord _record = new();

    /// <summary>
    ///     Трекер пайплайна
    /// </summary>
    public void Attach(Pipeline<TContext> pipeline, bool wasScheduled)
    {
        _record.PipelineName = pipeline.Name;
        _record.StartedAt = DateTime.UtcNow;
        _record.WasScheduled = wasScheduled;

        pipeline.OnStatusChanged += OnPipelineStatusChanged;
        pipeline.OnStepStatusChanged += OnStepStatusChanged;
    }

    /// <summary>
    ///     Получить запись
    /// </summary>
    public PipelineExecutionRecord GetRecord() => _record;

    private void OnStepStatusChanged(PipelineStepStatus status, string stepName, TContext context, Exception? exception)
    {
        var step = _record.Steps.FirstOrDefault(s => s.Name == stepName);
        if (step == null)
        {
            step = new StepExecutionInfo
            {
                Name = stepName,
                StartedAt = DateTime.UtcNow,
            };
            _record.Steps.Add(step);
        }

        step.Status = status;
        if (status is PipelineStepStatus.Failed or PipelineStepStatus.Completed)
        {
            step.FinishedAt = DateTime.UtcNow;
        }

        if (exception != null)
        {
            step.ErrorMessage = exception.Message;
        }
    }

    private void OnPipelineStatusChanged(PipelineStatus status, TContext context, Exception? exception)
    {
        _record.Status = status;
        _record.FinishedAt = DateTime.UtcNow;
        if (exception != null)
        {
            _record.ErrorMessage = exception.Message;
        }
    }
}
