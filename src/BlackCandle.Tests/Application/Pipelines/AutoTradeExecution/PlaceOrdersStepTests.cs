using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using Moq;

namespace BlackCandle.Tests.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Тесты на <see cref="PlaceOrdersStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Заявка размещена успешно</item>
///         <item>Заявка не прошла — ошибка</item>
///         <item>Логируется и успех, и ошибка</item>
///         <item>Обрабатываются все заявки</item>
///     </list>
/// </remarks>
public sealed class PlaceOrdersStepTests
{
    private readonly Mock<ITradingClient> _trading = new();
    private readonly Mock<IInvestApiFacade> _facade = new();
    private readonly Mock<ILoggerService> _logger = new();

    private readonly PlaceOrdersStep _step;

    public PlaceOrdersStepTests()
    {
        _facade.Setup(x => x.Trading).Returns(_trading.Object);
        _step = new PlaceOrdersStep(_facade.Object)
        {
            Logger = _logger.Object
        };
    }

    private static ExecutedTrade MakeTrade(string ticker = "AAPL") => new()
    {
        Ticker = new Ticker { Symbol = ticker },
        Quantity = 10,
        Side = TradeAction.Buy,
        Status = TradeStatus.Pending
    };

    /// <summary>
    ///     Тест 1: Заявка размещена успешно
    /// </summary>
    [Fact(DisplayName = "Тест 1: Заявка размещена успешно")]
    public async Task ExecuteAsync_ShouldMarkSuccess_WhenOrderPlaced()
    {
        var context = new AutoTradeExecutionContext
        {
            ExecutedTrades = [MakeTrade()]
        };

        _trading.Setup(x =>
                x.PlaceMarketOrderAsync(It.IsAny<Ticker>(), It.IsAny<decimal>(), It.IsAny<TradeAction>()))
            .ReturnsAsync(123.45m);

        await _step.ExecuteAsync(context);

        var trade = context.ExecutedTrades[0];
        Assert.Equal(TradeStatus.Success, trade.Status);
        Assert.Equal(123.45m, trade.Price);
    }

    /// <summary>
    ///     Тест 2: Заявка не прошла — ошибка
    /// </summary>
    [Fact(DisplayName = "Тест 2: Заявка не прошла — ошибка")]
    public async Task ExecuteAsync_ShouldMarkError_WhenExceptionThrown()
    {
        var context = new AutoTradeExecutionContext
        {
            ExecutedTrades = [MakeTrade()]
        };

        _trading.Setup(x =>
                x.PlaceMarketOrderAsync(It.IsAny<Ticker>(), It.IsAny<decimal>(), It.IsAny<TradeAction>()))
            .ThrowsAsync(new InvalidOperationException("fail"));

        await _step.ExecuteAsync(context);

        var trade = context.ExecutedTrades[0];
        Assert.Equal(TradeStatus.Error, trade.Status);
        Assert.Equal(0, trade.Price);
    }

    /// <summary>
    ///     Тест 3: Логируется и успех, и ошибка
    /// </summary>
    [Fact(DisplayName = "Тест 3: Логируется и успех, и ошибка")]
    public async Task ExecuteAsync_ShouldLogBothSuccessAndError()
    {
        var trades = new List<ExecutedTrade>
        {
            MakeTrade(),
            MakeTrade("FAIL")
        };

        _trading.Setup(x => x.PlaceMarketOrderAsync(
                It.Is< Ticker >(t => t.Symbol == "AAPL"), It.IsAny<decimal>(), It.IsAny<TradeAction>()))
            .ReturnsAsync(100);

        _trading.Setup(x => x.PlaceMarketOrderAsync(
                It.Is< Ticker >(t => t.Symbol == "FAIL"), It.IsAny<decimal>(), It.IsAny<TradeAction>()))
            .ThrowsAsync(new Exception("fail"));

        var context = new AutoTradeExecutionContext { ExecutedTrades = trades };

        await _step.ExecuteAsync(context);

        _logger.Verify(x => x.LogInfo(It.Is<string>(msg => msg.Contains("AAPL"))), Times.AtLeastOnce);
        _logger.Verify(x => x.LogError(It.Is<string>(msg => msg.Contains("FAIL")), It.IsAny<Exception>()), Times.Once);
    }

    /// <summary>
    ///     Тест 4: Обрабатываются все заявки
    /// </summary>
    [Fact(DisplayName = "Тест 4: Обрабатываются все заявки")]
    public async Task ExecuteAsync_ShouldProcessAllTrades()
    {
        var context = new AutoTradeExecutionContext
        {
            ExecutedTrades = [
                MakeTrade(),
                MakeTrade("SBER"),
                MakeTrade("YNDX")
            ]
        };

        _trading.Setup(c =>
                c.PlaceMarketOrderAsync(It.IsAny<Ticker>(), It.IsAny<decimal>(), It.IsAny<TradeAction>()))
            .ReturnsAsync(100);

        await _step.ExecuteAsync(context);

        Assert.All(context.ExecutedTrades, t =>
        {
            Assert.Equal(TradeStatus.Success, t.Status);
            Assert.Equal(100, t.Price);
        });
    }
}
