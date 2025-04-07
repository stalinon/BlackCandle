namespace BlackCandle.Domain.Configuration;

/// <summary>
///     Конфигурация Redis
/// </summary>
public sealed class RedisOptions
{
    /// <summary>
    ///     Использовать Redis
    /// </summary>
    public bool UseRedis { get; set; } = false;

    /// <summary>
    ///     Конфигурация
    /// </summary>
    public string Configuration { get; set; } = "localhost:6379";

    /// <summary>
    ///     Префикс
    /// </summary>
    public string Prefix { get; set; } = "blackcandle:";
}
