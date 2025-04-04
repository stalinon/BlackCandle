using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines;

/// <summary>
///     Делегат события смены статуса пайплайна
/// </summary>
public delegate void PipelineStatusChangedHandler<TContext>(
    PipelineStatus newStatus,
    TContext context,
    Exception? exception = null);
