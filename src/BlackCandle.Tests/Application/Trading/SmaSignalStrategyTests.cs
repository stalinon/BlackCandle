using BlackCandle.Application.Trading.SignalGeneration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

namespace BlackCandle.Tests.Application.Trading;

/// <summary>
///     Тесты на <see cref="SmaSignalStrategy" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Цена выше SMA — скор положительный</item>
///         <item>Цена ниже SMA — скор отрицательный</item>
///         <item>Цена равна SMA — скор 0</item>
///         <item>SMA без значения — null</item>
///         <item>Цена без значения — null</item>
///         <item>SMA отсутствует — null</item>
///         <item>Цена отсутствует — null</item>
///     </list>
/// </remarks>
public sealed class SmaSignalStrategyTests
{
    private readonly SmaSignalStrategy _strategy = new();

    /// <summary>
    ///     Тест 1: Цена выше SMA — скор положительный
    /// </summary>
    [Fact(DisplayName = "Тест 1: Цена выше SMA — скор положительный")]
    public void GenerateScore_ShouldReturnPositiveScore_WhenPriceAboveSma()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "SMA20", Value = 150, Date = DateTime.UtcNow,
            },
            new()
            {
                Name = "CLOSE", Value = 160, Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().BeGreaterThan(0);
        result.IndicatorName.Should().Be("SMA20");
        result.Reason.Should().Contain("против SMA");
    }

    /// <summary>
    ///     Тест 2: Цена ниже SMA — скор отрицательный
    /// </summary>
    [Fact(DisplayName = "Тест 2: Цена ниже SMA — скор отрицательный")]
    public void GenerateScore_ShouldReturnNegativeScore_WhenPriceBelowSma()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "TSLA" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "SMA20", Value = 180, Date = DateTime.UtcNow,
            },
            new()
            {
                Name = "CLOSE", Value = 170, Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().BeLessThan(0);
        result.Reason.Should().Contain("против SMA");
    }

    /// <summary>
    ///     Тест 3: Цена равна SMA — скор 0
    /// </summary>
    [Fact(DisplayName = "Тест 3: Цена равна SMA — скор 0")]
    public void GenerateScore_ShouldReturnZero_WhenPriceEqualsSma()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "MSFT" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "SMA20", Value = 100, Date = DateTime.UtcNow,
            },
            new()
            {
                Name = "CLOSE", Value = 100, Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().NotBeNull();
        result!.Score.Should().Be(0);
    }

    /// <summary>
    ///     Тест 4: SMA без значения — null
    /// </summary>
    [Fact(DisplayName = "Тест 4: SMA без значения — null")]
    public void GenerateScore_ShouldReturnNull_WhenSmaValueIsNull()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "NFLX" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "SMA20", Value = null, Date = DateTime.UtcNow,
            },
            new()
            {
                Name = "CLOSE", Value = 120, Date = DateTime.UtcNow,
            },
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
            new()
            {
                Name = "SMA20", Value = 120, Date = DateTime.UtcNow,
            },
            new()
            {
                Name = "CLOSE", Value = null, Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    ///     Тест 6: SMA отсутствует — null
    /// </summary>
    [Fact(DisplayName = "Тест 6: SMA отсутствует — null")]
    public void GenerateScore_ShouldReturnNull_WhenNoSmaIndicator()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "GOOG" };
        var indicators = new List<TechnicalIndicator>
        {
            new()
            {
                Name = "CLOSE",
                Value = 200,
                Date = DateTime.UtcNow,
            },
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
            new()
            {
                Name = "SMA20",
                Value = 110,
                Date = DateTime.UtcNow,
            },
        };

        // Act
        var result = _strategy.GenerateScore(ticker, indicators);

        // Assert
        result.Should().BeNull();
    }
}
