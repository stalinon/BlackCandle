using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Тесты на <see cref="CheckAutoTradePermissionStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Проверка разрешения: true — шаг выполняется</item>
///         <item>Проверка разрешения: false — вызывается EarlyExit</item>
///     </list>
/// </remarks>
public sealed class CheckAutoTradePermissionStepTests
{
    private readonly Mock<IBotSettingsService> _settings = new();

    private readonly CheckAutoTradePermissionStep _step;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckAutoTradePermissionStepTests"/> class.
    /// </summary>
    public CheckAutoTradePermissionStepTests()
    {
        _step = new CheckAutoTradePermissionStep(_settings.Object);
    }

    /// <summary>
    ///     Тест 1: Разрешение включено — шаг выполняется
    /// </summary>
    [Fact(DisplayName = "Тест 1: Разрешение включено — шаг выполняется")]
    public async Task ExecuteAsync_ShouldPass_WhenEnabled()
    {
        // Arrange
        _settings.Setup(x => x.GetAsync())
            .ReturnsAsync(new BotSettings { EnableAutoTrading = true });

        var context = new AutoTradeExecutionContext();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => _step.ExecuteAsync(context));
        Assert.Null(exception);
    }

    /// <summary>
    ///     Тест 2: Разрешение отключено — вызывается EarlyExit
    /// </summary>
    [Fact(DisplayName = "Тест 2: Разрешение отключено — вызывается EarlyExit")]
    public async Task ExecuteAsync_ShouldCallEarlyExit_WhenDisabled()
    {
        // Arrange
        _settings.Setup(x => x.GetAsync())
            .ReturnsAsync(new BotSettings { EnableAutoTrading = false });

        var context = new AutoTradeExecutionContext();
        var triggered = false;

        _step.EarlyExitAction += (_, ex, _) =>
        {
            triggered = true;
            Assert.IsType<AutoTradeNotEnabledException>(ex);
        };

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        Assert.True(triggered);
    }
}
