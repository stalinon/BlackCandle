using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="EvaluateTechnicalScoresStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Если индикаторы есть, стратегии вызываются</item>
///         <item>Если стратегия вернула null — пропускается</item>
///         <item>Если индикаторов нет — тикер игнорируется</item>
///     </list>
/// </remarks>
public sealed class EvaluateTechnicalScoresStepTests
{
    private readonly Mock<ISignalGenerationStrategy> _strategyMock = new();
    private readonly EvaluateTechnicalScoresStep _step;
    private readonly PortfolioAnalysisContext _context = new();

    /// <inheritdoc cref="EvaluateTechnicalScoresStepTests" />
    public EvaluateTechnicalScoresStepTests()
    {
        _step = new EvaluateTechnicalScoresStep([_strategyMock.Object]);
    }

    /// <summary>
    ///     Тест 1: Если индикаторы есть, стратегии вызываются
    /// </summary>
    [Fact(DisplayName = "Тест 1: Если индикаторы есть, стратегии вызываются")]
    public async Task ExecuteAsync_ShouldCallStrategy_WhenIndicatorsExist()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        _context.Tickers.Add(ticker);
        _context.Indicators[ticker] =
        [
            new()
            {
                Name = "RSI14",
                Value = 55,
                Date = DateTime.UtcNow,
            },
        ];

        var expectedScore = new TechnicalScore
        {
            IndicatorName = "RSI14",
            Score = 1,
            Value = 55,
            Reason = "Нормальный RSI",
        };

        _strategyMock
        .Setup(x => x.GenerateScore(ticker, It.IsAny<List<TechnicalIndicator>>()))
        .Returns(expectedScore);

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _strategyMock.Verify(
        x => x.GenerateScore(ticker, It.IsAny<List<TechnicalIndicator>>()),
        Times.Once);

        _context.TechnicalScores[ticker].Should().ContainSingle()
        .Which.Should().BeEquivalentTo(expectedScore);
    }

    /// <summary>
    ///     Тест 2: Если стратегия вернула null — пропускается
    /// </summary>
    [Fact(DisplayName = "Тест 2: Если стратегия вернула null — пропускается")]
    public async Task ExecuteAsync_ShouldSkipNullScores()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "MSFT" };
        _context.Tickers.Add(ticker);
        _context.Indicators[ticker] =
        [
            new()
            {
                Name = "MACD",
                Value = 0.5,
                Date = DateTime.UtcNow,
            },
        ];

        _strategyMock
            .Setup(x => x.GenerateScore(It.IsAny<Ticker>(), It.IsAny<List<TechnicalIndicator>>()))
            .Returns((TechnicalScore?)null);

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _context.TechnicalScores[ticker].Should().BeEmpty();
    }

    /// <summary>
    ///     Тест 3: Если индикаторов нет — тикер игнорируется
    /// </summary>
    [Fact(DisplayName = "Тест 3: Если индикаторов нет — тикер игнорируется")]
    public async Task ExecuteAsync_ShouldSkipTickerWithoutIndicators()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "GOOG" };
        _context.Tickers.Add(ticker); // но в Indicators не добавляем

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _strategyMock.Verify(
            x => x.GenerateScore(It.IsAny<Ticker>(), It.IsAny<List<TechnicalIndicator>>()),
            Times.Never);

        _context.TechnicalScores.Should().NotContainKey(ticker);
    }
}
