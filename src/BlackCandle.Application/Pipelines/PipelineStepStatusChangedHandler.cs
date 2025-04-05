using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines;

/// <summary>
///     Делегат события смены статуса шага пайплайна
/// </summary>
public delegate void PipelineStepStatusChangedHandler<TContext>(
    PipelineStepStatus newStatus,
    string stepName,
    TContext context,
    Exception? exception = null);
