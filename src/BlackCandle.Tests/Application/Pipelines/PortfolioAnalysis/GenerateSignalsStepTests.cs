using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="GenerateSignalsStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Генерация сигнала вызывается по тикерам с индикаторами</item>
///         <item>Сигналы добавляются, если не null</item>
///         <item>Сигналы не добавляются, если null</item>
///         <item>Тикеры без индикаторов игнорируются</item>
///     </list>
/// </remarks>
public sealed class GenerateSignalsStepTests
{
    private readonly Mock<IRepository<TradeSignal>> _signalRepoMock = new();
    private readonly Mock<IDataStorageContext> _storageMock = new();
    private readonly Mock<ISignalGenerationStrategy> _strategyMock = new();

    private readonly GenerateSignalsStep _step;
    private readonly PortfolioAnalysisContext _context = new();

    /// <inheritdoc cref="GenerateSignalsStepTests"/>
    public GenerateSignalsStepTests()
    {
        _storageMock.Setup(x => x.TradeSignals).Returns(_signalRepoMock.Object);

        _step = new GenerateSignalsStep(_storageMock.Object, _strategyMock.Object);
    }

    /// <summary>
    ///     Тест 1: Генерация сигнала вызывается по тикерам с индикаторами
    /// </summary>
    [Fact(DisplayName = "Тест 1: Генерация сигнала вызывается по тикерам с индикаторами")]
    public async Task ExecuteAsync_ShouldCallGenerate_WhenIndicatorsExist()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        _context.Tickers.Add(ticker);
        _context.Indicators[ticker] =
        [
            new() { Name = "RSI14", Value = 50, Date = DateTime.UtcNow }
        ];

        _strategyMock
            .Setup(x => x.Generate(ticker, It.IsAny<List<TechnicalIndicator>>(), 0, It.IsAny<DateTime>()))
            .Returns(new TradeSignal { Ticker = ticker, Action = TradeAction.Hold });

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _strategyMock.Verify(
            x => x.Generate(ticker, It.IsAny<List<TechnicalIndicator>>(), 0, It.IsAny<DateTime>()),
            Times.Once);
    }

    /// <summary>
    ///     Тест 2: Сигналы добавляются, если не null
    /// </summary>
    [Fact(DisplayName = "Тест 2: Сигналы добавляются, если не null")]
    public async Task ExecuteAsync_ShouldAddSignal_WhenNotNull()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "SBER" };
        _context.Tickers.Add(ticker);
        _context.Indicators[ticker] =
        [
            new() { Name = "MACD", Value = 1, Date = DateTime.UtcNow }
        ];

        var signal = new TradeSignal { Ticker = ticker, Action = TradeAction.Buy };

        _strategyMock
            .Setup(x => x.Generate(ticker, It.IsAny<List<TechnicalIndicator>>(), 0, It.IsAny<DateTime>()))
            .Returns(signal);

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _signalRepoMock.Verify(x => x.AddAsync(signal), Times.Once);
    }

    /// <summary>
    ///     Тест 3: Сигналы не добавляются, если null
    /// </summary>
    [Fact(DisplayName = "Тест 3: Сигналы не добавляются, если null")]
    public async Task ExecuteAsync_ShouldNotAddSignal_WhenNullReturned()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "LKOH" };
        _context.Tickers.Add(ticker);
        _context.Indicators[ticker] =
        [
            new() { Name = "ADX14", Value = 22, Date = DateTime.UtcNow }
        ];

        _strategyMock
            .Setup(x => x.Generate(ticker, It.IsAny<List<TechnicalIndicator>>(), 0, It.IsAny<DateTime>()))
            .Returns((TradeSignal?)null);

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _signalRepoMock.Verify(x => x.AddAsync(It.IsAny<TradeSignal>()), Times.Never);
    }

    /// <summary>
    ///     Тест 4: Тикеры без индикаторов игнорируются
    /// </summary>
    [Fact(DisplayName = "Тест 4: Тикеры без индикаторов игнорируются")]
    public async Task ExecuteAsync_ShouldSkipTickers_WithoutIndicators()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "GAZP" };
        _context.Tickers.Add(ticker); // но в Indicators словаре нет

        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _strategyMock.Verify(
            x => x.Generate(It.IsAny<Ticker>(), It.IsAny<List<TechnicalIndicator>>(), It.IsAny<int>(), It.IsAny<DateTime>()),
            Times.Never);
    }
}
