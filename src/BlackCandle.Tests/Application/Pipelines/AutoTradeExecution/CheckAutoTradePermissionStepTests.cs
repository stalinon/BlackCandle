using System.Linq.Expressions;
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
///         <item><description>Проверка разрешения: true — шаг выполняется</description></item>
///         <item><description>Проверка разрешения: false — вызывается EarlyExit</description></item>
///         <item><description>Ошибка, если в настройках больше одного элемента или ни одного</description></item>
///     </list>
/// </remarks>
public sealed class CheckAutoTradePermissionStepTests
{
    private readonly Mock<IRepository<BotSettings>> _settingsRepo = new();
    private readonly Mock<IDataStorageContext> _storage = new();

    private readonly CheckAutoTradePermissionStep _step;

    public CheckAutoTradePermissionStepTests()
    {
        _storage.Setup(x => x.BotSettings).Returns(_settingsRepo.Object);
        _step = new CheckAutoTradePermissionStep(_storage.Object);
    }

    /// <summary>
    ///     Тест 1: Разрешение включено — шаг выполняется
    /// </summary>
    [Fact(DisplayName = "Тест 1: Разрешение включено — шаг выполняется")]
    public async Task ExecuteAsync_ShouldPass_WhenEnabled()
    {
        // Arrange
        _settingsRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<BotSettings, bool>>>()))
            .ReturnsAsync([new BotSettings { EnableAutoTrading = true }]);

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
        _settingsRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<BotSettings, bool>>>()))
            .ReturnsAsync([new BotSettings { EnableAutoTrading = false }]);

        var context = new AutoTradeExecutionContext();
        var triggered = false;

        _step.EarlyExitAction += (ctx, ex, _) =>
        {
            triggered = true;
            Assert.IsType<AutoTradeNotEnabledException>(ex);
        };

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        Assert.True(triggered);
    }

    /// <summary>
    ///     Тест 3: Ошибка, если в настройках больше одного элемента или ни одного
    /// </summary>
    [Theory(DisplayName = "Тест 3: Ошибка, если в настройках больше одного элемента или ни одного")]
    [InlineData(0)]
    [InlineData(2)]
    public async Task ExecuteAsync_ShouldThrow_WhenSettingsAreInvalid(int count)
    {
        // Arrange
        var list = Enumerable.Range(1, count)
            .Select(_ => new BotSettings { EnableAutoTrading = true }).ToList();

        _settingsRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<BotSettings, bool>>>())).ReturnsAsync(list);

        var context = new AutoTradeExecutionContext();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _step.ExecuteAsync(context));
    }
}
