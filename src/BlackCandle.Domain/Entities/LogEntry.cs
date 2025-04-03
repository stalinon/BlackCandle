using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Лог записи
/// </summary>
public class LogEntry : IEntity
{
    /// <inheritdoc />
    public string Id => $"{Timestamp:yyyyMMddHHmmssfff}";
    
    /// <summary>
    ///     Момент времени
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    ///     Этап
    /// </summary>
    public string Step { get; set; } = string.Empty;

    /// <summary>
    ///     Сообщение
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    ///     Уровень (Info / Warning / Error)
    /// </summary>
    public string Level { get; set; } = "Info";
}