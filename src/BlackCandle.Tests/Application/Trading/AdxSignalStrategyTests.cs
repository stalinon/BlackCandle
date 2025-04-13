using BlackCandle.Application.Trading.SignalGeneration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

namespace BlackCandle.Tests.Application.Trading;

/// <summary>
///     Тесты на <see cref="AdxSignalStrategy" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>ADX > 25 — скор положительный</item>
///         <item>ADX между 20 и 25 — скор 0</item>
///         <item>ADX меньше 20 — скор 0</item>
///         <item>ADX без значения — null</item>
///         <item>ADX отсутствует — null</item>
///     </list>
/// </remarks>
public sealed class AdxSignalStrategyTests
{
    private readonly AdxSignalStrategy _strategy = new();

    /// <summary>
    ///     Тест 1: ADX > 25 — скор положительный
    /// </summary>
    [Fact(DisplayName = "Тест 1: ADX > 25 — скор положительный")]
    public void GenerateScore_ShouldReturnPositiveScore_WhenAdxAbove25()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "ADX14", Value = 30, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().BeGreaterThan(0);
        result.IndicatorName.Should().Be("ADX14");
        result.Reason.Should().Contain("ADX =");
    }

    /// <summary>
    ///     Тест 2: ADX между 20 и 25 — скор 0
    /// </summary>
    [Fact(DisplayName = "Тест 2: ADX между 20 и 25 — скор 0")]
    public void GenerateScore_ShouldReturnZero_WhenAdxBetween20And25()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "TSLA" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "ADX14", Value = 22, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().Be(0);
    }

    /// <summary>
    ///     Тест 3: ADX меньше 20 — скор 0
    /// </summary>
    [Fact(DisplayName = "Тест 3: ADX < 20 — скор 0")]
    public void GenerateScore_ShouldReturnZero_WhenAdxBelow20()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "GOOG" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "ADX14", Value = 15, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().Be(0);
    }

    /// <summary>
    ///     Тест 4: ADX без значения — null
    /// </summary>
    [Fact(DisplayName = "Тест 4: ADX без значения — null")]
    public void GenerateScore_ShouldReturnNull_WhenAdxValueIsNull()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "NFLX" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "ADX14", Value = null, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Тест 5: ADX отсутствует — null
    /// </summary>
    [Fact(DisplayName = "Тест 5: ADX отсутствует — null")]
    public void GenerateScore_ShouldReturnNull_WhenNoAdxIndicator()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "MSFT" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "RSI14", Value = 50, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }
}
