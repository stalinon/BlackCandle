namespace BlackCandle.Domain.Enums;

/// <summary>
///     Статус пайплайна
/// </summary>
public enum PipelineStatus
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
