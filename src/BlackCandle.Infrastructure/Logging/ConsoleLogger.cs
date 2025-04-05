using BlackCandle.Application.Interfaces.Infrastructure;

namespace BlackCandle.Infrastructure.Logging;

/// <summary>
///     Простейший логгер. Только для ранней разработки и дебага.
/// </summary>
internal sealed class ConsoleLogger : ILoggerService
{
    private readonly List<string> _prefixes = [];

    private string Preffix => string.Join(" ", _prefixes.Select(x => $"[{x}]"));

    /// <inheritdoc />
    public void AddPrefix(string prefix)
    {
        if (_prefixes.Contains(prefix))
        {
            return;
        }

        _prefixes.Add(prefix);
    }

    /// <inheritdoc />
    public void LogInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"[INFO] {Preffix} {DateTime.Now}: {message}");
        Console.ResetColor();
    }

    /// <inheritdoc />
    public void LogWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARN] {Preffix} {DateTime.Now}: {message}");
        Console.ResetColor();
    }

    /// <inheritdoc />s
    public void LogError(string message, Exception? ex = null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {Preffix} {DateTime.Now}: {message}");
        Console.WriteLine($"        {message}");
        if (ex != null)
        {
            Console.WriteLine($"        {ex.Message}");
            Console.WriteLine($"        {ex.StackTrace}");
        }

        Console.ResetColor();
    }
}
