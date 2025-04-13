using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Infrastructure.Trading;

using Moq;

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

    /// <inheritdoc cref="TradeExecutionServiceTests"/>
    public TradeExecutionServiceTests()
    {
        _facadeMock.Setup(x => x.Marketdata).Returns(_marketMock.Object);

        var config = new TradeExecutionOptions
        {
            MaxTradeAmountRub = 10000m,
            MaxLotsPerTrade = 5,
        };

        var botSettingsServiceMock = new Mock<IBotSettingsService>();
        botSettingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new BotSettings()
        {
            TradeExecution = config,
        });

        _service = new TradeExecutionService(_facadeMock.Object, botSettingsServiceMock.Object);
    }

    /// <summary>
    ///     Тест 1: Сигнал Hold всегда даёт объём 0
    /// </summary>
    [Fact(DisplayName = "Тест 1: Сигнал Hold всегда даёт объём 0")]
    public async Task CalculateVolume_ShouldReturnZero_WhenActionIsHold()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Hold };

        // Act
        var volume = await _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(0, volume);
    }

    /// <summary>
    ///     Тест 2: Нулевая или отрицательная цена — объём 0
    /// </summary>
    [Theory(DisplayName = "Тест 2: Нулевая или отрицательная цена — объём 0")]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task CalculateVolume_ShouldReturnZero_WhenPriceInvalid(decimal price)
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "TEST" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(price);

        // Act
        var volume = await _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(0, volume);
    }

    /// <summary>
    ///     Тест 3: Слишком маленький бюджет — объём 0
    /// </summary>
    [Fact(DisplayName = "Тест 3: Слишком маленький бюджет — объём 0")]
    public async Task CalculateVolume_ShouldReturnZero_WhenBudgetTooSmall()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "TEST" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(20_000m);

        // Act
        var volume = await _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(0, volume);
    }

    /// <summary>
    ///     Тест 4: Ограничение по MaxLotsPerTrade
    /// </summary>
    [Fact(DisplayName = "Тест 4: Ограничение по MaxLotsPerTrade")]
    public async Task CalculateVolume_ShouldRespectMaxLotsLimit()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "TEST" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(100m); // rawQty = 100

        // Act
        var volume = await _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(5, volume); // MaxLotsPerTrade
    }

    /// <summary>
    ///     Тест 5: Корректный сигнал даёт правильный объём
    /// </summary>
    [Fact(DisplayName = "Тест 5: Корректный сигнал даёт правильный объём")]
    public async Task CalculateVolume_ShouldReturnExpectedVolume()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "TEST" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(2500m); // rawQty = 4

        // Act
        var volume = await _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(4, volume);
    }

    /// <summary>
    ///     Тест 6: Используется AllocatedCash вместо лимита из настроек
    /// </summary>
    [Fact(DisplayName = "Тест 6: Используется AllocatedCash вместо лимита из настроек")]
    public async Task CalculateVolume_ShouldUseAllocatedCash_IfSet()
    {
        // Arrange
        var signal = new TradeSignal
        {
            Action = TradeAction.Buy,
            Ticker = new Ticker { Symbol = "TEST" },
            AllocatedCash = 3000m,
        };

        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(1000m); // rawQty = 3

        // Act
        var volume = await _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(3, volume);
    }

    /// <summary>
    ///     Тест 7: AllocatedCash даёт объём, превышающий MaxLots — ограничивает
    /// </summary>
    [Fact(DisplayName = "Тест 7: AllocatedCash даёт объём, превышающий MaxLots — ограничивает")]
    public async Task CalculateVolume_ShouldLimitVolume_WhenAllocatedExceedsMax()
    {
        // Arrange
        var signal = new TradeSignal
        {
            Action = TradeAction.Buy,
            Ticker = new Ticker { Symbol = "TEST" },
            AllocatedCash = 1_000_000m,
        };

        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(10m); // rawQty = 100000

        // Act
        var volume = await _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(5, volume); // MaxLotsPerTrade = 5
    }

    /// <summary>
    ///     Тест 8: AllocatedCash меньше цены — объём 0
    /// </summary>
    [Fact(DisplayName = "Тест 8: AllocatedCash меньше цены — объём 0")]
    public async Task CalculateVolume_ShouldReturnZero_WhenAllocatedCashTooLow()
    {
        // Arrange
        var signal = new TradeSignal
        {
            Action = TradeAction.Buy,
            Ticker = new Ticker { Symbol = "TEST" },
            AllocatedCash = 300m,
        };

        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(1000m); // rawQty = 0

        // Act
        var volume = await _service.CalculateVolume(signal);

        // Assert
        Assert.Equal(0, volume);
    }
}
