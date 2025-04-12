using BlackCandle.Application.Interfaces.Infrastructure;

namespace BlackCandle.Tests;

/// <summary>
///     Логгер
/// </summary>
internal sealed class Logger : ILoggerService
{
    /// <inheritdoc />
    public void AddPrefix(string prefix)
    {
    }

    /// <inheritdoc />
    public void LogInfo(string message)
    {
    }

    /// <inheritdoc />
    public void LogWarning(string message)
    {
    }

    /// <inheritdoc />
    public void LogError(string message, Exception? ex = null)
    {
    }
}
