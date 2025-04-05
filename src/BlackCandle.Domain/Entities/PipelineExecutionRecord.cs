using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Запись о исполнении пайплайна
/// </summary>
public class PipelineExecutionRecord : IEntity
{
    /// <inheritdoc />
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    ///     Название пайплайна
    /// </summary>
    public string PipelineName { get; set; } = default!;

    /// <summary>
    ///     Дата старта
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    ///     Дата окончания
    /// </summary>
    public DateTime? FinishedAt { get; set; }

    /// <summary>
    ///     Статус пайплайна
    /// </summary>
    public PipelineStatus Status { get; set; }

    /// <summary>
    ///     Был запущен в ручную или по расписанию
    /// </summary>
    public bool WasScheduled { get; set; }

    /// <summary>
    ///     Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    ///     Шаги
    /// </summary>
    public List<StepExecutionInfo> Steps { get; set; } = new();
}
