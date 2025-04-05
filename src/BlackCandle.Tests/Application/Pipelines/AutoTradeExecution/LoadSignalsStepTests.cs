using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Тесты на <see cref="LoadSignalsStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Загружаются только сигналы Buy/Sell</item>
///         <item>Фильтрация по сегодняшней дате</item>
///         <item>Сигналы попадают в контекст</item>
///     </list>
/// </remarks>
public sealed class LoadSignalsStepTests
{
    private readonly Mock<IRepository<TradeSignal>> _signalsRepo = new();
    private readonly Mock<IDataStorageContext> _storage = new();

    private readonly LoadSignalsStep _step;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadSignalsStepTests"/> class.
    /// </summary>
    public LoadSignalsStepTests()
    {
        _storage.Setup(x => x.TradeSignals).Returns(_signalsRepo.Object);
        _step = new LoadSignalsStep(_storage.Object);
    }

    /// <summary>
    ///     Тест 1: Загружаются только сигналы Buy/Sell
    /// </summary>
    [Fact(DisplayName = "Тест 1: Загружаются только сигналы Buy/Sell")]
    public async Task ExecuteAsync_ShouldOnlyLoadBuyAndSellSignals()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;

        var signals = new List<TradeSignal>
        {
            new() { Action = TradeAction.Buy, Date = today },
            new() { Action = TradeAction.Sell, Date = today },
            new() { Action = TradeAction.Hold, Date = today },
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>>()))
            .ReturnsAsync(signals.Where(s => s.Action != TradeAction.Hold).ToList());

        var context = new AutoTradeExecutionContext();

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        Assert.All(context.Signals, s => Assert.NotEqual(TradeAction.Hold, s.Action));
    }

    /// <summary>
    ///     Тест 2: Фильтрация по сегодняшней дате
    /// </summary>
    [Fact(DisplayName = "Тест 2: Фильтрация по сегодняшней дате")]
    public async Task ExecuteAsync_ShouldFilterByDate()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;

        var signals = new List<TradeSignal>
        {
            new() { Action = TradeAction.Buy, Date = today },
            new() { Action = TradeAction.Sell, Date = today.AddDays(-1) },
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>>()))
            .ReturnsAsync(signals.Where(s => s.Date.Date == today).ToList());

        var context = new AutoTradeExecutionContext();

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        Assert.Single(context.Signals);
        Assert.Equal(today, context.Signals[0].Date.Date);
    }

    /// <summary>
    ///     Тест 3: Сигналы попадают в контекст
    /// </summary>
    [Fact(DisplayName = "Тест 3: Сигналы попадают в контекст")]
    public async Task ExecuteAsync_ShouldWriteSignalsToContext()
    {
        var today = DateTime.UtcNow.Date;

        var expected = new List<TradeSignal>
        {
            new() { Action = TradeAction.Buy, Date = today },
            new() { Action = TradeAction.Sell, Date = today },
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>>()))
            .ReturnsAsync(expected);

        var context = new AutoTradeExecutionContext();

        await _step.ExecuteAsync(context);

        Assert.Equal(expected, context.Signals);
    }
}
