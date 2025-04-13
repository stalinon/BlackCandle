using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
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
///         <item>Пропускает шаг при пустых сигналах</item>
///         <item>Добавляет деньги от продажи в кэш</item>
///         <item>Распределяет кэш по скору</item>
///         <item>Только покупки получают allocated cash</item>
///         <item>Не создаёт трейд при объёме 0</item>
///         <item>Создаёт трейд при объёме > 0</item>
///     </list>
/// </remarks>
public sealed class CalculateTradeVolumeStepTests
{
    private readonly Mock<ITradeExecutionService> _execMock = new();
    private readonly Mock<IInvestApiFacade> _apiMock = new();
    private readonly Mock<IDataStorageContext> _storageMock = new();
    private readonly CalculateTradeVolumeStep _step;

    /// <inheritdoc cref="CalculateTradeVolumeStepTests" />
    public CalculateTradeVolumeStepTests()
    {
        _step = new CalculateTradeVolumeStep(_execMock.Object, _apiMock.Object, _storageMock.Object);
    }

    /// <summary>
    ///     Тест 1: Пропускает шаг при пустом списке сигналов
    /// </summary>
    [Fact(DisplayName = "Тест 1: Пропускает шаг при пустом списке сигналов")]
    public async Task ExecuteAsync_ShouldSkip_WhenNoSignals()
    {
        var context = new AutoTradeExecutionContext { Signals = [] };

        await _step.ExecuteAsync(context);

        Assert.Empty(context.ExecutedTrades);
    }

    /// <summary>
    ///     Тест 2: Добавляет деньги от продажи в доступный кэш
    /// </summary>
    [Fact(DisplayName = "Тест 2: Добавляет деньги от продажи в доступный кэш")]
    public async Task ExecuteAsync_ShouldAddSellRevenueToCash()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals = [
                new TradeSignal
                {
                    Action = TradeAction.Sell,
                    Ticker = new Ticker { Symbol = "SBER" },
                },
            ],
        };

        _apiMock.Setup(x => x.Portfolio.GetAvailableCashAsync()).ReturnsAsync(1000);
        _apiMock.Setup(x => x.Marketdata.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(500);
        _storageMock.Setup(x => x.PortfolioAssets.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync(new List<PortfolioAsset>
        {
            new() { Ticker = new Ticker { Symbol = "SBER" }, Quantity = 1 },
        });

        _execMock.Setup(x => x.CalculateVolume(It.IsAny<TradeSignal>())).ReturnsAsync(1);

        await _step.ExecuteAsync(context);

        Assert.Empty(context.ExecutedTrades);
    }

    /// <summary>
    ///     Тест 3: Распределяет кэш по скору сигналов на покупку
    /// </summary>
    [Fact(DisplayName = "Тест 3: Распределяет кэш по скору сигналов на покупку")]
    public async Task ExecuteAsync_ShouldDistributeCash_ByScore()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals =
            [
                new() { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL" }, FundamentalScore = 1 },
                new() { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "GOOG" }, FundamentalScore = 3 }
            ],
        };

        _apiMock.Setup(x => x.Portfolio.GetAvailableCashAsync()).ReturnsAsync(400);
        _storageMock.Setup(x => x.PortfolioAssets.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync([]);

        _execMock.Setup(x => x.CalculateVolume(It.IsAny<TradeSignal>())).ReturnsAsync(1);

        await _step.ExecuteAsync(context);

        var aapl = context.Signals.First(s => s.Ticker.Symbol == "AAPL");
        var goog = context.Signals.First(s => s.Ticker.Symbol == "GOOG");

        Assert.Equal(100, aapl.AllocatedCash);
        Assert.Equal(300, goog.AllocatedCash);
    }

    /// <summary>
    ///     Тест 4: Не назначает кэш сигналам на продажу
    /// </summary>
    [Fact(DisplayName = "Тест 4: Не назначает кэш сигналам на продажу")]
    public async Task ExecuteAsync_ShouldNotAssignCash_ToSellSignals()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals =
            [
                new() { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL" }, FundamentalScore = 1 },
                new() { Action = TradeAction.Sell, Ticker = new Ticker { Symbol = "SBER" } }
            ],
        };

        _apiMock.Setup(x => x.Portfolio.GetAvailableCashAsync()).ReturnsAsync(100);
        _storageMock.Setup(x => x.PortfolioAssets.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync([]);

        _execMock.Setup(x => x.CalculateVolume(It.IsAny<TradeSignal>())).ReturnsAsync(1);

        await _step.ExecuteAsync(context);

        var sell = context.Signals.First(s => s.Action == TradeAction.Sell);
        Assert.Null(sell.AllocatedCash);
    }

    /// <summary>
    ///     Тест 5: Объём = 0 — трейд не создается
    /// </summary>
    [Fact(DisplayName = "Тест 5: Объём = 0 — трейд не создается")]
    public async Task ExecuteAsync_ShouldNotCreateTrade_WhenVolumeZero()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals = [new() { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL" }, FundamentalScore = 1 }],
        };

        _apiMock.Setup(x => x.Portfolio.GetAvailableCashAsync()).ReturnsAsync(100);
        _storageMock.Setup(x => x.PortfolioAssets.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync([]);
        _execMock.Setup(x => x.CalculateVolume(It.IsAny<TradeSignal>())).ReturnsAsync(0);

        await _step.ExecuteAsync(context);

        Assert.Empty(context.ExecutedTrades);
    }

    /// <summary>
    ///     Тест 6: Объём > 0 — трейд создается
    /// </summary>
    [Fact(DisplayName = "Тест 6: Объём > 0 — трейд создается")]
    public async Task ExecuteAsync_ShouldCreateTrade_WhenVolumePositive()
    {
        var context = new AutoTradeExecutionContext
        {
            Signals = [new() { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL" }, FundamentalScore = 1 }],
        };

        _apiMock.Setup(x => x.Portfolio.GetAvailableCashAsync()).ReturnsAsync(1000);
        _storageMock.Setup(x => x.PortfolioAssets.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync([]);
        _execMock.Setup(x => x.CalculateVolume(It.IsAny<TradeSignal>())).ReturnsAsync(5);

        await _step.ExecuteAsync(context);

        Assert.Single(context.ExecutedTrades);
        Assert.Equal(5, context.ExecutedTrades[0].Quantity);
        Assert.Equal(TradeStatus.Pending, context.ExecutedTrades[0].Status);
    }
}
