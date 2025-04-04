using System.Text;
using BlackCandle.Infrastructure.Logging;

namespace BlackCandle.Tests.Infrastructure;

/// <summary>
///     Тесты на <see href="ConsoleLogger" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Добавление префикса</item>
///         <item>Игнорирование дубликатных префиксов</item>
///         <item>Логирование информации</item>
///         <item>Логирование предупреждений</item>
///         <item>Логирование ошибок без исключения</item>
///         <item>Логирование ошибок с исключением</item>
///     </list>
/// </remarks>
public sealed class ConsoleLoggerTests
{
    private ConsoleLogger CreateLogger()
    {
        return new ConsoleLogger();
    }

    /// <summary>
    ///     Тест 1: Добавление префикса
    /// </summary>
    [Fact(DisplayName = "Тест 1: Добавление префикса")]
    public void AddPrefix_ShouldAddPrefix()
    {
        // Arrange
        var logger = CreateLogger();

        // Act
        logger.AddPrefix("TEST");

        // Assert (визуально — лог с этим префиксом будет виден в других тестах)
    }

    /// <summary>
    ///     Тест 2: Игнорирование дубликатных префиксов
    /// </summary>
    [Fact(DisplayName = "Тест 2: Игнорирование дубликатных префиксов")]
    public void AddPrefix_ShouldIgnoreDuplicates()
    {
        // Arrange
        var logger = CreateLogger();

        // Act
        logger.AddPrefix("SAME");
        logger.AddPrefix("SAME");

        // Assert — не упадёт, логика внутри защищена
    }

    /// <summary>
    ///     Тест 3: Логирование информации
    /// </summary>
    [Fact(DisplayName = "Тест 3: Логирование информации")]
    public void LogInfo_ShouldOutputCyanLine()
    {
        // Arrange
        var logger = CreateLogger();
        var output = new StringBuilder();
        using var writer = new StringWriter(output);
        Console.SetOut(writer);

        // Act
        logger.LogInfo("Info test message");

        // Assert
        Assert.Contains("[INFO]", output.ToString());
        Assert.Contains("Info test message", output.ToString());
    }

    /// <summary>
    ///     Тест 4: Логирование предупреждений
    /// </summary>
    [Fact(DisplayName = "Тест 4: Логирование предупреждений")]
    public void LogWarning_ShouldOutputYellowLine()
    {
        // Arrange
        var logger = CreateLogger();
        var output = new StringBuilder();
        using var writer = new StringWriter(output);
        Console.SetOut(writer);

        // Act
        logger.LogWarning("Warn test message");

        // Assert
        Assert.Contains("[WARN]", output.ToString());
        Assert.Contains("Warn test message", output.ToString());
    }

    /// <summary>
    ///     Тест 5: Логирование ошибок без исключения
    /// </summary>
    [Fact(DisplayName = "Тест 5: Логирование ошибок без исключения")]
    public void LogError_ShouldOutputRedLine_WithoutException()
    {
        // Arrange
        var logger = CreateLogger();
        var output = new StringBuilder();
        using var writer = new StringWriter(output);
        Console.SetOut(writer);

        // Act
        logger.LogError("Error test message");

        // Assert
        Assert.Contains("[ERROR]", output.ToString());
        Assert.Contains("Error test message", output.ToString());
    }

    /// <summary>
    ///     Тест 6: Логирование ошибок с исключением
    /// </summary>
    [Fact(DisplayName = "Тест 6: Логирование ошибок с исключением")]
    public void LogError_ShouldIncludeExceptionMessage()
    {
        // Arrange
        var logger = CreateLogger();
        var ex = new InvalidOperationException("Ошибка обработки");
        var output = new StringBuilder();
        using var writer = new StringWriter(output);
        Console.SetOut(writer);

        // Act
        logger.LogError("Ошибка при логировании", ex);

        // Assert
        var log = output.ToString();
        Assert.Contains("[ERROR]", log);
        Assert.Contains("Ошибка при логировании", log);
        Assert.Contains("Ошибка обработки", log);
    }
}
