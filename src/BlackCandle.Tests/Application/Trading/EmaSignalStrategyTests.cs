using BlackCandle.Application.Trading.SignalGeneration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

namespace BlackCandle.Tests.Application.Trading;

/// <summary>
///     Тесты на <see cref="EmaSignalStrategy" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Цена выше EMA — скор положительный</item>
///         <item>Цена ниже EMA — скор отрицательный</item>
///         <item>Цена равна EMA — скор 0</item>
///         <item>EMA без значения — null</item>
///         <item>Цена без значения — null</item>
///         <item>EMA отсутствует — null</item>
///         <item>Цена отсутствует — null</item>
///     </list>
/// </remarks>
public sealed class EmaSignalStrategyTests
{
    private readonly EmaSignalStrategy _strategy = new();

    /// <summary>
    ///     Тест 1: Цена выше EMA — скор положительный
    /// </summary>
    [Fact(DisplayName = "Тест 1: Цена выше EMA — скор положительный")]
    public void GenerateScore_ShouldReturnPositiveScore_WhenPriceAboveEma()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "EMA12", Value = 150, Date = DateTime.UtcNow },
            new() { Name = "CLOSE", Value = 160, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().BeGreaterThan(0);
        result.IndicatorName.Should().Be("EMA12");
        result.Reason.Should().Contain("против EMA");
    }

    /// <summary>
    ///     Тест 2: Цена ниже EMA — скор отрицательный
    /// </summary>
    [Fact(DisplayName = "Тест 2: Цена ниже EMA — скор отрицательный")]
    public void GenerateScore_ShouldReturnNegativeScore_WhenPriceBelowEma()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "TSLA" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "EMA12", Value = 180, Date = DateTime.UtcNow },
            new() { Name = "CLOSE", Value = 170, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().BeLessThan(0);
        result.Reason.Should().Contain("против EMA");
    }

    /// <summary>
    ///     Тест 3: Цена равна EMA — скор 0
    /// </summary>
    [Fact(DisplayName = "Тест 3: Цена равна EMA — скор 0")]
    public void GenerateScore_ShouldReturnZero_WhenPriceEqualsEma()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "MSFT" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "EMA12", Value = 100, Date = DateTime.UtcNow },
            new() { Name = "CLOSE", Value = 100, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().Be(0);
    }

    /// <summary>
    ///     Тест 4: EMA без значения — null
    /// </summary>
    [Fact(DisplayName = "Тест 4: EMA без значения — null")]
    public void GenerateScore_ShouldReturnNull_WhenEmaValueIsNull()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "NFLX" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "EMA12", Value = null, Date = DateTime.UtcNow },
            new() { Name = "CLOSE", Value = 120, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Тест 5: Цена без значения — null
    /// </summary>
    [Fact(DisplayName = "Тест 5: Цена без значения — null")]
    public void GenerateScore_ShouldReturnNull_WhenCloseValueIsNull()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "NVDA" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "EMA12", Value = 120, Date = DateTime.UtcNow },
            new() { Name = "CLOSE", Value = null, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Тест 6: EMA отсутствует — null
    /// </summary>
    [Fact(DisplayName = "Тест 6: EMA отсутствует — null")]
    public void GenerateScore_ShouldReturnNull_WhenNoEmaIndicator()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "GOOG" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "CLOSE", Value = 200, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Тест 7: Цена отсутствует — null
    /// </summary>
    [Fact(DisplayName = "Тест 7: Цена отсутствует — null")]
    public void GenerateScore_ShouldReturnNull_WhenNoCloseIndicator()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "INTC" };
        var indicators = new List<TechnicalIndicator>
        {
            new() { Name = "EMA12", Value = 110, Date = DateTime.UtcNow },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }
}
