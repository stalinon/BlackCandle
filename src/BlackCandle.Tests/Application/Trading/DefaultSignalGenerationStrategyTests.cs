using BlackCandle.Application.Trading.SignalGeneration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

namespace BlackCandle.Tests.Application.Trading;

/// <summary>
///     Тесты на <see cref="DefaultSignalGenerationStrategy" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Возвращает null при нехватке индикаторов</item>
///         <item>Генерирует Buy при RSI меньше 40 и EMA > SMA</item>
///         <item>Генерирует Sell при RSI > 60 и EMA меньше SMA</item>
///         <item>Генерирует Hold при прочих условиях</item>
///         <item>Выставляет Medium confidence при score >= 3</item>
///     </list>
/// </remarks>
public sealed class DefaultSignalGenerationStrategyTests
{
    private readonly DefaultSignalGenerationStrategy _strategy = new();
    private readonly Ticker _ticker = new() { Symbol = "TEST" };
    private readonly DateTime _now = new(2024, 1, 1);

    /// <summary>
    ///     Тест 1: Возвращает null при нехватке индикаторов
    /// </summary>
    [Fact(DisplayName = "Тест 1: Возвращает null при нехватке индикаторов")]
    public void Generate_ShouldReturnNull_WhenIndicatorsMissing()
    {
        // Arrange
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "RSI14", Value = 35, Date = _now },

            // Нет EMA, SMA, ADX, MACD
        };

        // Act
        var result = _strategy.Generate(_ticker, indicators, 0, _now);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Тест 2: Генерирует Buy при RSI меньше 40 и EMA > SMA
    /// </summary>
    [Fact(DisplayName = "Тест 2: Генерирует Buy при RSI < 40 и EMA > SMA")]
    public void Generate_ShouldReturnBuySignal_WhenRsiLowAndEmaAboveSma()
    {
        // Arrange
        var indicators = GetIndicators(rsi: 35, ema: 105, sma: 100, macd: 1, adx: 25);

        // Act
        var result = _strategy.Generate(_ticker, indicators, 1, _now);

        // Assert
        result.Should().NotBeNull();
        result!.Action.Should().Be(TradeAction.Buy);
        result.Confidence.Should().Be(ConfidenceLevel.Low);
    }

    /// <summary>
    ///     Тест 3: Генерирует Sell при RSI > 60 и EMA меньше SMA
    /// </summary>
    [Fact(DisplayName = "Тест 3: Генерирует Sell при RSI > 60 и EMA < SMA")]
    public void Generate_ShouldReturnSellSignal_WhenRsiHighAndEmaBelowSma()
    {
        var indicators = GetIndicators(rsi: 65, ema: 95, sma: 100, macd: 1, adx: 25);

        var result = _strategy.Generate(_ticker, indicators, 1, _now);

        result.Should().NotBeNull();
        result!.Action.Should().Be(TradeAction.Sell);
        result.Confidence.Should().Be(ConfidenceLevel.Low);
    }

    /// <summary>
    ///     Тест 4: Генерирует Hold, если условия не выполнены
    /// </summary>
    [Fact(DisplayName = "Тест 4: Генерирует Hold, если условия не выполнены")]
    public void Generate_ShouldReturnHold_WhenConditionsNotMet()
    {
        var indicators = GetIndicators(rsi: 50, ema: 100, sma: 100, macd: 1, adx: 25);

        var result = _strategy.Generate(_ticker, indicators, 1, _now);

        result.Should().NotBeNull();
        result!.Action.Should().Be(TradeAction.Hold);
    }

    /// <summary>
    ///     Тест 5: Выставляет Confidence = Medium при score >= 3
    /// </summary>
    [Fact(DisplayName = "Тест 5: Выставляет Confidence = Medium при score >= 3")]
    public void Generate_ShouldSetMediumConfidence_WhenScoreHigh()
    {
        var indicators = GetIndicators(rsi: 35, ema: 105, sma: 100, macd: 1, adx: 25);

        var result = _strategy.Generate(_ticker, indicators, 3, _now);

        result.Should().NotBeNull();
        result!.Confidence.Should().Be(ConfidenceLevel.Medium);
    }

    /// <summary>
    ///     Тест 6: Возвращает все ключевые поля заполненными
    /// </summary>
    [Fact(DisplayName = "Тест 6: Возвращает все ключевые поля заполненными")]
    public void Generate_ShouldPopulateAllFields()
    {
        var indicators = GetIndicators(rsi: 35, ema: 105, sma: 100, macd: 1, adx: 25);

        var result = _strategy.Generate(_ticker, indicators, 2, _now);

        result.Should().NotBeNull();
        result!.Ticker.Should().Be(_ticker);
        result.Date.Should().Be(_now);
        result.Score.Should().Be(2);
        result.Reason.Should().Contain("RSI=");
        result.Reason.Should().Contain("EMA=");
        result.Reason.Should().Contain("Score=2");
    }

    private List<TechnicalIndicator> GetIndicators(double rsi, double ema, double sma, double macd, double adx)
    {
        return
        [
            new() { Name = "RSI14", Value = rsi, Date = _now },
            new() { Name = "EMA12", Value = ema, Date = _now },
            new() { Name = "SMA20", Value = sma, Date = _now },
            new() { Name = "MACD", Value = macd, Date = _now },
            new() { Name = "ADX14", Value = adx, Date = _now }
        ];
    }
}
