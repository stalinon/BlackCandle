using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="GenerateSignalsStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Генерация сигнала вызывается при наличии фундаментального и технических скорингов</item>
///         <item>Сигнал не создаётся без фундаментального скоринга</item>
///         <item>Сигнал не создаётся без технических скоров</item>
///     </list>
/// </remarks>
public sealed class GenerateSignalsStepTests
{
    private readonly Mock<IRepository<TradeSignal>> _signalRepoMock = new();
    private readonly Mock<IDataStorageContext> _storageMock = new();
    private readonly GenerateSignalsStep _step;
    private readonly PortfolioAnalysisContext _context = new();

    /// <inheritdoc cref="GenerateSignalsStepTests" />
    public GenerateSignalsStepTests()
    {
        _storageMock.SetupGet(x => x.TradeSignals).Returns(_signalRepoMock.Object);
        _step = new GenerateSignalsStep(_storageMock.Object);
    }

    /// <summary>
    ///     Тест 1: Генерация сигнала вызывается при наличии фундаментального и технических скорингов
    /// </summary>
    [Fact(DisplayName = "Тест 1: Генерация сигнала вызывается при наличии фундаментального и технических скорингов")]
    public async Task ExecuteAsync_ShouldGenerateSignal_WhenScoresExist()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        _context.Tickers.Add(ticker);
        _context.FundamentalScores[ticker] = 3;
        _context.TechnicalScores[ticker] =
        [
            new()
            {
                IndicatorName = "RSI14",
                Score = 2,
                Value = 25,
                Reason = "RSI < 30",
            },
        ];

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _signalRepoMock.Verify(
        x => x.AddAsync(It.Is<TradeSignal>(s =>
                s.Ticker.Symbol == "AAPL" &&
                s.FundamentalScore == 3 &&
                s.TechnicalScores.Count == 1)),
        Times.Once);
    }

    /// <summary>
    ///     Тест 2: Сигнал не создаётся без фундаментального скоринга
    /// </summary>
    [Fact(DisplayName = "Тест 2: Сигнал не создаётся без фундаментального скоринга")]
    public async Task ExecuteAsync_ShouldNotGenerate_WhenNoFundamentalScore()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "MSFT" };
        _context.Tickers.Add(ticker);
        _context.TechnicalScores[ticker] =
        [
            new()
            {
                IndicatorName = "MACD",
                Score = 1,
                Value = 0.8,
                Reason = "MACD > 0",
            },
        ];

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _signalRepoMock.Verify(x => x.AddAsync(It.IsAny<TradeSignal>()), Times.Never);
    }

    /// <summary>
    ///     Тест 3: Сигнал не создаётся без технических скоров
    /// </summary>
    [Fact(DisplayName = "Тест 3: Сигнал не создаётся без технических скоров")]
    public async Task ExecuteAsync_ShouldNotGenerate_WhenNoTechnicalScores()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "GOOG" };
        _context.Tickers.Add(ticker);
        _context.FundamentalScores[ticker] = 2;

        // технические скоры отсутствуют

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _signalRepoMock.Verify(x => x.AddAsync(It.IsAny<TradeSignal>()), Times.Never);
    }
}
