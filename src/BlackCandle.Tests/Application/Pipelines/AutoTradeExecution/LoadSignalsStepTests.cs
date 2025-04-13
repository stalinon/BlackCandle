using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Тесты на <see cref="LoadSignalsStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Отфильтровывает Hold-сигналы</item>
///         <item>Отфильтровывает сигналы с Confidence = Low</item>
///         <item>Сигналы Buy/Sell и Confidence != Low проходят</item>
///         <item>Фильтрация по сегодняшней дате</item>
///     </list>
/// </remarks>
public sealed class LoadSignalsStepTests
{
    private readonly Mock<IRepository<TradeSignal>> _signalsRepo = new();
    private readonly Mock<IDataStorageContext> _storage = new();
    private readonly LoadSignalsStep _step;

    /// <inheritdoc cref="LoadSignalsStepTests" />
    public LoadSignalsStepTests()
    {
        _storage.Setup(x => x.TradeSignals).Returns(_signalsRepo.Object);
        _step = new LoadSignalsStep(_storage.Object);
    }

    /// <summary>
    ///     Тест 1: Отфильтровывает Hold-сигналы
    /// </summary>
    [Fact(DisplayName = "Тест 1: Отфильтровывает Hold-сигналы")]
    public async Task ExecuteAsync_ShouldExcludeHoldSignals()
    {
        var today = DateTime.UtcNow.Date;

        var signals = new List<TradeSignal>
        {
            new() { Action = TradeAction.Buy, Date = today },
            new() { Action = TradeAction.Sell, Date = today },
            new() { Action = TradeAction.Hold, Date = today },
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<IFilter<TradeSignal>>()))
            .ReturnsAsync(signals);

        var context = new AutoTradeExecutionContext();

        await _step.ExecuteAsync(context);

        Assert.All(context.Signals, s => Assert.NotEqual(TradeAction.Hold, s.Action));
    }

    /// <summary>
    ///     Тест 2: Отфильтровывает сигналы с Confidence = Low
    /// </summary>
    [Fact(DisplayName = "Тест 2: Отфильтровывает сигналы с Confidence = Low")]
    public async Task ExecuteAsync_ShouldExcludeLowConfidence()
    {
        var today = DateTime.UtcNow.Date;

        var signals = new List<TradeSignal>
        {
            new() { Action = TradeAction.Buy, Confidence = ConfidenceLevel.High, Date = today },
            new() { Action = TradeAction.Buy, Confidence = ConfidenceLevel.Low, Date = today },
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<IFilter<TradeSignal>>()))
            .ReturnsAsync(signals);

        var context = new AutoTradeExecutionContext();

        await _step.ExecuteAsync(context);

        Assert.Single(context.Signals);
        Assert.Equal(ConfidenceLevel.High, context.Signals[0].Confidence);
    }

    /// <summary>
    ///     Тест 3: Проходят только Buy/Sell + Confidence != Low
    /// </summary>
    [Fact(DisplayName = "Тест 3: Проходят только Buy/Sell + Confidence != Low")]
    public async Task ExecuteAsync_ShouldKeepBuySellAndNonLowConfidence()
    {
        var today = DateTime.UtcNow.Date;

        var signals = new List<TradeSignal>
        {
            new() { Action = TradeAction.Buy, Confidence = ConfidenceLevel.Medium, Date = today },
            new() { Action = TradeAction.Sell, Confidence = ConfidenceLevel.High, Date = today },
            new() { Action = TradeAction.Hold, Confidence = ConfidenceLevel.High, Date = today },
            new() { Action = TradeAction.Buy, Confidence = ConfidenceLevel.Low, Date = today },
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<IFilter<TradeSignal>>()))
            .ReturnsAsync(signals);

        var context = new AutoTradeExecutionContext();

        await _step.ExecuteAsync(context);

        Assert.Equal(2, context.Signals.Count);
        Assert.All(context.Signals, s =>
        {
            Assert.NotEqual(TradeAction.Hold, s.Action);
            Assert.NotEqual(ConfidenceLevel.Low, s.Confidence);
        });
    }

    /// <summary>
    ///     Тест 4: Фильтрация по сегодняшней дате
    /// </summary>
    [Fact(DisplayName = "Тест 4: Фильтрация по сегодняшней дате")]
    public async Task ExecuteAsync_ShouldFilterByDate()
    {
        var today = DateTime.UtcNow.Date;

        var signals = new List<TradeSignal>
        {
            new() { Action = TradeAction.Buy, Date = today },
            new() { Action = TradeAction.Buy, Date = today.AddDays(-1) },
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<IFilter<TradeSignal>>()))
            .ReturnsAsync(signals.Where(x => x.Date == today).ToList());

        var context = new AutoTradeExecutionContext();

        await _step.ExecuteAsync(context);

        Assert.All(context.Signals, s => Assert.Equal(today, s.Date));
    }
}
