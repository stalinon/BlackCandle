using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Infrastructure.Trading;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BlackCandle.Tests.Infrastructure.Trading;

/// <summary>
///     Тесты на <see cref="TradeExecutionService" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Сигнал Hold всегда даёт объём 0</item>
///         <item>Нулевая или отрицательная цена — объём 0</item>
///         <item>Слишком маленький бюджет — объём 0</item>
///         <item>Ограничение по MaxLotsPerTrade</item>
///         <item>Корректный сигнал даёт правильный объём</item>
///     </list>
/// </remarks>
public sealed class TradeExecutionServiceTests
{
    private readonly Mock<IInvestApiFacade> _facadeMock = new();
    private readonly Mock<IMarketDataClient> _marketMock = new();

    private readonly ITradeExecutionService _service;

    public TradeExecutionServiceTests()
    {
        _facadeMock.Setup(x => x.Marketdata).Returns(_marketMock.Object);

        var config = Options.Create(new TradeExecutionOptions
        {
            MaxTradeAmountRub = 10000m,
            MaxLotsPerTrade = 5
        });

        _service = new TradeExecutionService(_facadeMock.Object, config);
    }

    /// <summary>
    ///     Тест 1: Сигнал Hold всегда даёт объём 0
    /// </summary>
    [Fact(DisplayName = "Тест 1: Сигнал Hold всегда даёт объём 0")]
    public void CalculateVolume_ShouldReturnZero_WhenActionIsHold()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Hold };

        // Act
        var volume = _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(0, volume);
    }

    /// <summary>
    ///     Тест 2: Нулевая или отрицательная цена — объём 0
    /// </summary>
    [Theory(DisplayName = "Тест 2: Нулевая или отрицательная цена — объём 0")]
    [InlineData(0)]
    [InlineData(-100)]
    public void CalculateVolume_ShouldReturnZero_WhenPriceInvalid(decimal price)
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "TEST" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(price);

        // Act
        var volume = _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(0, volume);
    }

    /// <summary>
    ///     Тест 3: Слишком маленький бюджет — объём 0
    /// </summary>
    [Fact(DisplayName = "Тест 3: Слишком маленький бюджет — объём 0")]
    public void CalculateVolume_ShouldReturnZero_WhenBudgetTooSmall()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "TEST" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(20_000m);

        // Act
        var volume = _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(0, volume);
    }

    /// <summary>
    ///     Тест 4: Ограничение по MaxLotsPerTrade
    /// </summary>
    [Fact(DisplayName = "Тест 4: Ограничение по MaxLotsPerTrade")]
    public void CalculateVolume_ShouldRespectMaxLotsLimit()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "TEST" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(100m); // rawQty = 100

        // Act
        var volume = _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(5, volume); // MaxLotsPerTrade
    }

    /// <summary>
    ///     Тест 5: Корректный сигнал даёт правильный объём
    /// </summary>
    [Fact(DisplayName = "Тест 5: Корректный сигнал даёт правильный объём")]
    public void CalculateVolume_ShouldReturnExpectedVolume()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "TEST" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(2500m); // rawQty = 4

        // Act
        var volume = _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(4, volume);
    }
}
