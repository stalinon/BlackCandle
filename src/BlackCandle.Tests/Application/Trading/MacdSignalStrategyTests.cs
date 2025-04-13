using BlackCandle.Application.Trading.SignalGeneration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

namespace BlackCandle.Tests.Application.Trading;

/// <summary>
///     Тесты на <see cref="MacdSignalStrategy" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>MACD > 0 — скор положительный</item>
///         <item>MACD меньше 0 — скор отрицательный</item>
///         <item>MACD == 0 — скор 0</item>
///         <item>MACD без значения — null</item>
///         <item>MACD отсутствует — null</item>
///     </list>
/// </remarks>
public sealed class MacdSignalStrategyTests
{
    private readonly MacdSignalStrategy _strategy = new();

    /// <summary>
    ///     Тест 1: MACD > 0 — скор положительный
    /// </summary>
    [Fact(DisplayName = "Тест 1: MACD > 0 — скор положительный")]
    public void GenerateScore_ShouldReturnPositiveScore_WhenMacdAboveZero()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "MACD", Value = 1.25, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().BeGreaterThan(0);
        result.IndicatorName.Should().Be("MACD");
        result.Reason.Should().Contain("MACD =");
    }

    /// <summary>
    ///     Тест 2: MACD меньше 0 — скор отрицательный
    /// </summary>
    [Fact(DisplayName = "Тест 2: MACD < 0 — скор отрицательный")]
    public void GenerateScore_ShouldReturnNegativeScore_WhenMacdBelowZero()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "TSLA" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "MACD", Value = -0.77, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().BeLessThan(0);
        result.Reason.Should().Contain("MACD");
    }

    /// <summary>
    ///     Тест 3: MACD == 0 — скор 0
    /// </summary>
    [Fact(DisplayName = "Тест 3: MACD == 0 — скор 0")]
    public void GenerateScore_ShouldReturnZeroScore_WhenMacdIsZero()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "MSFT" };
        var indicators = new List<TechnicalIndicator> { new() { Name = "MACD", Value = 0, Date = DateTime.UtcNow }, };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().Be(0);
    }

    /// <summary>
    ///     Тест 4: MACD без значения — null
    /// </summary>
    [Fact(DisplayName = "Тест 4: MACD без значения — null")]
    public void GenerateScore_ShouldReturnNull_WhenMacdValueIsNull()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "NFLX" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "MACD", Value = null, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Тест 5: MACD отсутствует — null
    /// </summary>
    [Fact(DisplayName = "Тест 5: MACD отсутствует — null")]
    public void GenerateScore_ShouldReturnNull_WhenNoMacdIndicator()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "GOOG" };
        var indicators = new List<TechnicalIndicator> { new() { Name = "RSI14", Value = 55, Date = DateTime.UtcNow }, };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }
}
