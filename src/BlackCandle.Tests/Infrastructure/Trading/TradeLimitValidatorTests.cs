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
///     Тесты на <see cref="TradeLimitValidator" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Игнорируется не-Buy сигнал</item>
///         <item>Отклонение при нулевой/отрицательной цене</item>
///         <item>Отклонение при цене ниже минимальной</item>
///         <item>Отклонение при превышении доли в портфеле</item>
///         <item>Успешная валидация корректного сигнала</item>
///         <item>Отклонение при превышении лимита бумаг в секторе</item>
///         <item>Успешная валидация при недостижении лимита сектора</item>
///     </list>
/// </remarks>
public sealed class TradeLimitValidatorTests
{
    private readonly Mock<IInvestApiFacade> _apiMock = new();
    private readonly Mock<IMarketDataClient> _marketMock = new();
    private readonly ITradeLimitValidator _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeLimitValidatorTests"/> class.
    /// </summary>
    public TradeLimitValidatorTests()
    {
        _apiMock.Setup(x => x.Marketdata).Returns(_marketMock.Object);

        var options = new TradeLimitOptions
        {
            MinTradeAmountRub = 1000m,
            MaxPositionSharePercent = 50m,
        };

        var botSettingsServiceMock = new Mock<IBotSettingsService>();
        botSettingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new BotSettings()
        {
            TradeLimit = options,
        });

        _validator = new TradeLimitValidator(_apiMock.Object, botSettingsServiceMock.Object);
    }

    /// <summary>
    ///     Тест 1: Игнорируется не-Buy сигнал
    /// </summary>
    [Fact(DisplayName = "Тест 1: Игнорируется не-Buy сигнал")]
    public async Task Validate_ShouldReturnTrue_ForNonBuy()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Sell };

        // Act
        var result = await _validator.Validate(signal, []);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    ///     Тест 2: Отклонение при нулевой или отрицательной цене
    /// </summary>
    [Theory(DisplayName = "Тест 2: Отклонение при нулевой или отрицательной цене")]
    [InlineData(null)]
    [InlineData(0.0)]
    [InlineData(-5.0)]
    public async Task Validate_ShouldReject_WhenPriceInvalid(double? price)
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync((decimal?)price);

        // Act
        var result = await _validator.Validate(signal, []);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    ///     Тест 3: Отклонение при цене ниже минимальной
    /// </summary>
    [Fact(DisplayName = "Тест 3: Отклонение при цене ниже минимальной")]
    public async Task Validate_ShouldReject_WhenPriceBelowMinimum()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(999);

        // Act
        var result = await _validator.Validate(signal, []);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    ///     Тест 4: Отклонение при превышении доли позиции
    /// </summary>
    [Fact(DisplayName = "Тест 4: Отклонение при превышении доли позиции")]
    public async Task Validate_ShouldReject_WhenShareTooBig()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(10_000);

        var portfolio = new List<PortfolioAsset>
        {
            new() { Ticker = new Ticker { Symbol = "SBER" }, Quantity = 1, CurrentValue = 10_000 },
        };

        // Act
        var result = await _validator.Validate(signal, portfolio);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    ///     Тест 5: Успешная валидация при корректных условиях
    /// </summary>
    [Fact(DisplayName = "Тест 5: Успешная валидация при корректных условиях")]
    public async Task Validate_ShouldReturnTrue_WhenWithinLimits()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(1_000);

        var portfolio = new List<PortfolioAsset>
        {
            new() { Ticker = new Ticker { Symbol = "SBER" }, Quantity = 10, CurrentValue = 1_000 },
        };

        // Act
        var result = await _validator.Validate(signal, portfolio);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    ///     Тест 6: Отклонение при превышении лимита бумаг в секторе
    /// </summary>
    [Fact(DisplayName = "Тест 6: Отклонение при превышении лимита бумаг в секторе")]
    public async Task Validate_ShouldReject_WhenSectorLimitExceeded()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL", Sector = "Finance" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(1_000);

        var portfolio = new List<PortfolioAsset>
        {
            new() { Ticker = new Ticker { Symbol = "SBER", Sector = "Finance" }, Quantity = 1, CurrentValue = 1_000 },
            new() { Ticker = new Ticker { Symbol = "VTBR", Sector = "Finance" }, Quantity = 1, CurrentValue = 1_000 },
        };

        // Force сектор в текущем сигнале такой же
        portfolio.Add(new PortfolioAsset
        {
            Ticker = new Ticker { Symbol = "AAPL", Sector = "Finance" },
            Quantity = 0,
            CurrentValue = 0,
        });

        var options = new TradeLimitOptions
        {
            MinTradeAmountRub = 1000m,
            MaxPositionSharePercent = 50m,
            MaxSectorPositions = 2,
        };

        var botSettingsServiceMock = new Mock<IBotSettingsService>();
        botSettingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new BotSettings
        {
            TradeLimit = options,
        });

        var validator = new TradeLimitValidator(_apiMock.Object, botSettingsServiceMock.Object);

        // Act
        var result = await validator.Validate(signal, portfolio);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    ///     Тест 7: Успешная валидация при недостижении лимита сектора
    /// </summary>
    [Fact(DisplayName = "Тест 7: Успешная валидация при недостижении лимита сектора")]
    public async Task Validate_ShouldAllow_WhenSectorLimitNotReached()
    {
        // Arrange
        var signal = new TradeSignal { Action = TradeAction.Buy, Ticker = new Ticker { Symbol = "AAPL", Sector = "Finance" } };
        _marketMock.Setup(x => x.GetCurrentPriceAsync(It.IsAny<Ticker>())).ReturnsAsync(1_000);

        var portfolio = new List<PortfolioAsset>
        {
            new() { Ticker = new Ticker { Symbol = "SBER", Sector = "Finance" }, Quantity = 1, CurrentValue = 1_000 },
        };

        portfolio.Add(new PortfolioAsset
        {
            Ticker = new Ticker { Symbol = "AAPL", Sector = "Finance" },
            Quantity = 0,
            CurrentValue = 0,
        });

        var options = new TradeLimitOptions
        {
            MinTradeAmountRub = 1000m,
            MaxPositionSharePercent = 100m,
            MaxSectorPositions = 2,
        };

        var botSettingsServiceMock = new Mock<IBotSettingsService>();
        botSettingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new BotSettings
        {
            TradeLimit = options,
        });

        var validator = new TradeLimitValidator(_apiMock.Object, botSettingsServiceMock.Object);

        // Act
        var result = await validator.Validate(signal, portfolio);

        // Assert
        Assert.True(result);
    }
}
