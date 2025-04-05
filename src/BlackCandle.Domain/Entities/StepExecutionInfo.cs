using BlackCandle.Domain.Enums;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Информация о шаге пайплайна
/// </summary>
public class StepExecutionInfo
{
    /// <summary>
    ///     Название пайплайна
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    ///     Дата запуска
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    ///     Дата окончания
    /// </summary>
    public DateTime? FinishedAt { get; set; }

    /// <summary>
    ///     Статус
    /// </summary>
    public PipelineStepStatus Status { get; set; }

    /// <summary>
    ///     Ошибка
    /// </summary>
    public string? ErrorMessage { get; set; }
}
