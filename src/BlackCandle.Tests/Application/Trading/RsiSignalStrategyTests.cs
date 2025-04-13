using BlackCandle.Application.Trading.SignalGeneration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

namespace BlackCandle.Tests.Application.Trading;

/// <summary>
///     Тесты на <see cref="RsiSignalStrategy" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>RSI меньше 30 → скор 2</item>
///         <item>RSI > 70 → скор -2</item>
///         <item>RSI в норме → скор 0</item>
///         <item>Нет значения RSI → null</item>
///         <item>Нет индикатора RSI → null</item>
///     </list>
/// </remarks>
public sealed class RsiSignalStrategyTests
{
    private readonly RsiSignalStrategy _strategy = new();

    /// <summary>
    ///     Тест 1: RSI меньше 30 → скор 2
    /// </summary>
    [Fact(DisplayName = "Тест 1: RSI < 30 → скор 2")]
    public void GenerateScore_ShouldReturnScore2_WhenRsiBelow30()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "RSI14",
                Value = 25,
                Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().Be(2);
        result.IndicatorName.Should().Be("RSI14");
        result.Reason.Should().Contain("перепроданность");
    }

    /// <summary>
    ///     Тест 2: RSI > 70 → скор -2
    /// </summary>
    [Fact(DisplayName = "Тест 2: RSI > 70 → скор -2")]
    public void GenerateScore_ShouldReturnMinus2_WhenRsiAbove70()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "TSLA" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "RSI14",
                Value = 75,
                Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().Be(-2);
        result.Reason.Should().Contain("перекупленность");
    }

    /// <summary>
    ///     Тест 3: RSI в норме → скор 0
    /// </summary>
    [Fact(DisplayName = "Тест 3: RSI в норме → скор 0")]
    public void GenerateScore_ShouldReturnZero_WhenRsiNormal()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "MSFT" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "RSI14",
                Value = 50,
                Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().Be(0);
        result.Reason.Should().Contain("RSI в норме");
    }

    /// <summary>
    ///     Тест 4: RSI без значения → null
    /// </summary>
    [Fact(DisplayName = "Тест 4: RSI без значения → null")]
    public void GenerateScore_ShouldReturnNull_WhenValueIsNull()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "NFLX" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "RSI14",
                Value = null,
                Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Тест 5: Нет индикатора RSI → null
    /// </summary>
    [Fact(DisplayName = "Тест 5: Нет индикатора RSI → null")]
    public void GenerateScore_ShouldReturnNull_WhenNoRsiIndicator()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "GOOG" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "MACD",
                Value = 1.1,
                Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }
}
