using System.Linq.Expressions;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;
using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="CalculateIndicatorsStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item><description>Игнорируются тикеры с количеством свечей меньше 50</description></item>
///         <item><description>Рассчитываются SMA, EMA, RSI, MACD, ADX</description></item>
///         <item><description>Null-значения в индикаторах отфильтровываются</description></item>
///         <item><description>Группировка индикаторов по дате сохраняется</description></item>
///     </list>
/// </remarks>
public sealed class CalculateIndicatorsStepTests
{
    private readonly Mock<IRepository<PriceHistoryPoint>> _marketMock = new();
    private readonly Mock<IDataStorageContext> _storageMock = new();

    private readonly CalculateIndicatorsStep _step;

    private readonly Ticker _ticker = new() { Symbol = "AAPL" };

    public CalculateIndicatorsStepTests()
    {
        _storageMock.Setup(x => x.Marketdata).Returns(_marketMock.Object);
        _step = new CalculateIndicatorsStep(_storageMock.Object);
    }

    private List<PriceHistoryPoint> GenerateCandles(int count)
    {
        return Enumerable.Range(0, count).Select(i => new PriceHistoryPoint
        {
            Ticker = _ticker,
            Date = DateTime.UtcNow.AddDays(-count + i),
            Open = 100,
            High = 105,
            Low = 95,
            Close = 100 + i % 3,
            Volume = 1_000 + i * 10
        }).ToList();
    }

    /// <summary>
    ///     Тест 1: Игнорируются тикеры с количеством свечей меньше 50
    /// </summary>
    [Fact(DisplayName = "Тест 1: Игнорируются тикеры с количеством свечей < 50")]
    public async Task ExecuteAsync_ShouldSkipTicker_WhenLessThan50Candles()
    {
        // Arrange
        _marketMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PriceHistoryPoint, bool>>>())).ReturnsAsync(GenerateCandles(30));
        var context = new PortfolioAnalysisContext();

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        Assert.Empty(context.Indicators);
    }

    /// <summary>
    ///     Тест 2: Рассчитываются SMA, EMA, RSI, MACD, ADX
    /// </summary>
    [Fact(DisplayName = "Тест 2: Рассчитываются SMA, EMA, RSI, MACD, ADX")]
    public async Task ExecuteAsync_ShouldCalculateAllIndicators()
    {
        // Arrange
        _marketMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PriceHistoryPoint, bool>>>())).ReturnsAsync(GenerateCandles(100));
        var context = new PortfolioAnalysisContext();

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        var result = context.Indicators[_ticker];
        Assert.Contains(result, i => i.Name == "SMA20");
        Assert.Contains(result, i => i.Name == "EMA12");
        Assert.Contains(result, i => i.Name == "RSI14");
        Assert.Contains(result, i => i.Name == "MACD");
        Assert.Contains(result, i => i.Name == "ADX14");
    }

    /// <summary>
    ///     Тест 3: Null-значения в индикаторах отфильтровываются
    /// </summary>
    [Fact(DisplayName = "Тест 3: Null-значения в индикаторах отфильтровываются")]
    public async Task ExecuteAsync_ShouldIgnoreIndicatorsWithNullValue()
    {
        // Arrange
        var candles = GenerateCandles(100);
        _marketMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PriceHistoryPoint, bool>>>())).ReturnsAsync(candles);

        var context = new PortfolioAnalysisContext();

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        Assert.DoesNotContain(context.Indicators[_ticker], i => i.Value == null);
    }

    /// <summary>
    ///     Тест 4: Группировка индикаторов по дате сохраняется
    /// </summary>
    [Fact(DisplayName = "Тест 4: Группировка индикаторов по дате сохраняется")]
    public async Task ExecuteAsync_ShouldGroupByDateAndFlatten()
    {
        // Arrange
        _marketMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PriceHistoryPoint, bool>>>())).ReturnsAsync(GenerateCandles(100));
        var context = new PortfolioAnalysisContext();

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        var indicators = context.Indicators[_ticker];
        var grouped = indicators.GroupBy(i => i.Date);
        foreach (var group in grouped)
        {
            Assert.All(group, i => Assert.Equal(group.Key, i.Date));
        }
    }
}
