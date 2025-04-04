namespace BlackCandle.Application.Interfaces;

/// <summary>
///     Универсальный логгер для всего пайплайна: от API до бэкенда.
///     Реализация может вести лог в файл, консоль, Logtail, БД и т.д.
/// </summary>
public interface ILoggerService
{
    /// <summary>
    ///     Добавить префикс
    /// </summary>
    void AddPrefix(string prefix);
    
    /// <summary>
    ///     Записывает информационное сообщение.
    /// </summary>
    void LogInfo(string message);

    /// <summary>
    ///     Записывает предупреждение.
    /// </summary>
    void LogWarning(string message);

    /// <summary>
    ///     Записывает сообщение об ошибке.
    /// </summary>
    void LogError(string message, Exception? ex = null);
}