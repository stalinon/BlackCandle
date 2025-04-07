using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Запись лога для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisLogEntry : IStorageEntity<LogEntry>
{
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

    /// <inheritdoc />
    public LogEntry ToEntity() => new()
    {
        Timestamp = Timestamp,
        Step = Step,
        Message = Message,
        Level = Level,
    };

    /// <inheritdoc />
    public IStorageEntity<LogEntry> ToStorageEntity(LogEntry entity)
    {
        return new RedisLogEntry
        {
            Timestamp = entity.Timestamp,
            Step = entity.Step,
            Message = entity.Message,
            Level = entity.Level,
        };
    }
}
