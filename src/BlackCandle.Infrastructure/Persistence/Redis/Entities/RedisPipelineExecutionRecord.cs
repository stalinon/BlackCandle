using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Запись о выполнении пайплайна для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisPipelineExecutionRecord : IStorageEntity<PipelineExecutionRecord>
{
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
    ///     Был запущен вручную или по расписанию
    /// </summary>
    public bool WasScheduled { get; set; }

    /// <summary>
    ///     Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    ///     Этапы выполнения
    /// </summary>
    public List<StepExecutionInfo> Steps { get; set; } = new();

    /// <inheritdoc />
    public PipelineExecutionRecord ToEntity() => new()
    {
        PipelineName = PipelineName,
        StartedAt = StartedAt,
        FinishedAt = FinishedAt,
        Status = Status,
        WasScheduled = WasScheduled,
        ErrorMessage = ErrorMessage,
        Steps = Steps,
    };

    /// <inheritdoc />
    public IStorageEntity<PipelineExecutionRecord> ToStorageEntity(PipelineExecutionRecord entity)
    {
        return new RedisPipelineExecutionRecord
        {
            PipelineName = entity.PipelineName,
            StartedAt = entity.StartedAt,
            FinishedAt = entity.FinishedAt,
            Status = entity.Status,
            WasScheduled = entity.WasScheduled,
            ErrorMessage = entity.ErrorMessage,
            Steps = entity.Steps,
        };
    }
}
