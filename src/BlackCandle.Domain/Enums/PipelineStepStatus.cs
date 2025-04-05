namespace BlackCandle.Domain.Enums;

/// <summary>
///     Статус шага пайплайна
/// </summary>
public enum PipelineStepStatus
{
    /// <summary>
    ///     Не запущен
    /// </summary>
    NotStarted,

    /// <summary>
    ///     Запущен
    /// </summary>
    Running,

    /// <summary>
    ///     Возникла ошибка
    /// </summary>
    Failed,

    /// <summary>
    ///     Окончен
    /// </summary>
    Completed,
}
