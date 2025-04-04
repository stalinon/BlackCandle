using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using Moq;

namespace BlackCandle.Tests.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Тесты на <see cref="CalculateTradeVolumeStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Объём 0 — трейд не создается</item>
///         <item>Объём > 0 — трейд создается</item>
///         <item>Создаются только валидные трейды</item>
///         <item>У всех трейдов статус Pending</item>
///     </list>
/// </remarks>
public sealed class CalculateTradeVolumeStepTests
{
    private readonly Mock<ITradeExecutionService> _exec = new();
    private readonly CalculateTradeVolumeStep _step;

    public CalculateTradeVolumeStepTests()
    {
        _step = new CalculateTradeVolumeStep(_exec.Object);
    }

    /// <summary>
    ///     Тест 1: Объём 0 — трейд не создается
    /// </summary>
    [Fact(DisplayName = "Тест 1: Объём 0 — трейд не создается")]
    public async Task ExecuteAsync_ShouldSkipTrade_WhenVolumeIsZero()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals = [new TradeSignal { Ticker = new Ticker { Symbol = "AAPL" } }]
        };

        _exec.Setup(x => x.CalculateVolume(It.IsAny<TradeSignal>())).Returns(0);

        await _step.ExecuteAsync(context);

        Assert.Empty(context.ExecutedTrades);
    }

    /// <summary>
    ///     Тест 2: Объём > 0 — трейд создается
    /// </summary>
    [Fact(DisplayName = "Тест 2: Объём > 0 — трейд создается")]
    public async Task ExecuteAsync_ShouldCreateTrade_WhenVolumeIsPositive()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals = [new TradeSignal { Ticker = new Ticker { Symbol = "AAPL" } }]
        };

        _exec.Setup(x => x.CalculateVolume(It.IsAny<TradeSignal>())).Returns(10);

        await _step.ExecuteAsync(context);

        Assert.Single(context.ExecutedTrades);
        Assert.Equal("AAPL", context.ExecutedTrades[0].Ticker.Symbol);
        Assert.Equal(10, context.ExecutedTrades[0].Quantity);
    }

    /// <summary>
    ///     Тест 3: Создаются только валидные трейды
    /// </summary>
    [Fact(DisplayName = "Тест 3: Создаются только валидные трейды")]
    public async Task ExecuteAsync_ShouldFilterValidTrades()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals = [
                new TradeSignal { Ticker = new Ticker { Symbol = "AAPL" } },
                new TradeSignal { Ticker = new Ticker { Symbol = "SBER" } }
            ]
        };

        _exec.Setup(x => x.CalculateVolume(It.Is<TradeSignal>(s => s.Ticker.Symbol == "AAPL"))).Returns(10);
        _exec.Setup(x => x.CalculateVolume(It.Is<TradeSignal>(s => s.Ticker.Symbol == "SBER"))).Returns(0);

        await _step.ExecuteAsync(context);

        Assert.Single(context.ExecutedTrades);
        Assert.Equal("AAPL", context.ExecutedTrades[0].Ticker.Symbol);
    }

    /// <summary>
    ///     Тест 4: У всех трейдов статус Pending
    /// </summary>
    [Fact(DisplayName = "Тест 4: У всех трейдов статус Pending")]
    public async Task ExecuteAsync_ShouldSetPendingStatus()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals = [
                new TradeSignal { Ticker = new Ticker { Symbol = "AAPL" } },
                new TradeSignal { Ticker = new Ticker { Symbol = "SBER" } }
            ]
        };

        _exec.Setup(s => s.CalculateVolume(It.IsAny<TradeSignal>())).Returns(1);

        await _step.ExecuteAsync(context);

        Assert.All(context.ExecutedTrades, t => Assert.Equal(TradeStatus.Pending, t.Status));
    }
}
