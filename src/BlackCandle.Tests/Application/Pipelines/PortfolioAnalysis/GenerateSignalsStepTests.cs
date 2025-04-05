using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
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
///         <item>Не генерируется сигнал без RSI</item>
///         <item>Buy (Low) при RSI меньше 30 без подтверждений</item>
///         <item>Buy (Medium) при RSI меньше 30 и слабом score</item>
///         <item>Buy (High) при RSI меньше 30 и сильном score</item>
///         <item>Sell (Medium) при RSI > 70 без подтверждений</item>
///         <item>Sell (High) при RSI > 70 и подтверждении</item>
///         <item>Hold при RSI в пределах нормы</item>
///     </list>
/// </remarks>
public sealed class GenerateSignalsStepTests
{
    private readonly Mock<IRepository<TradeSignal>> _signalRepo = new();
    private readonly Mock<IRepository<PortfolioAsset>> _portfolioRepo = new();
    private readonly Mock<IDataStorageContext> _storage = new();

    private readonly Ticker _ticker = new() { Symbol = "AAPL" };

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateSignalsStepTests"/> class.
    /// </summary>
    public GenerateSignalsStepTests()
    {
        _storage.Setup(x => x.TradeSignals).Returns(_signalRepo.Object);
        _storage.Setup(x => x.PortfolioAssets).Returns(_portfolioRepo.Object);

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync([new PortfolioAsset { Ticker = _ticker }]);
    }

    /// <summary>
    ///     Тест 1: Не генерируется сигнал без RSI
    /// </summary>
    [Fact(DisplayName = "Тест 1: Не генерируется сигнал без RSI")]
    public async Task ExecuteAsync_ShouldNotGenerateSignal_WhenNoRSI()
    {
        var context = new PortfolioAnalysisContext
        {
            Indicators = new Dictionary<Ticker, List<TechnicalIndicator>>
            { [_ticker] = BuildIndicators(("MACD", 1), ("EMA12", 10), ("SMA20", 9), ("ADX14", 25)) },
            FundamentalScores = new Dictionary<Ticker, int> { [_ticker] = 5 },
        };

        var step = new GenerateSignalsStep(_storage.Object);

        await step.ExecuteAsync(context);

        _signalRepo.Verify(x => x.AddAsync(It.IsAny<TradeSignal>()), Times.Never);
    }

    /// <summary>
    ///     Тест 2: Buy (Low) при RSI меньше 30 без подтверждений
    /// </summary>
    [Fact(DisplayName = "Тест 2: Buy (Low) при RSI < 30 без подтверждений")]
    public async Task ExecuteAsync_ShouldGenerateBuyLowSignal()
    {
        var context = new PortfolioAnalysisContext
        {
            Indicators = new Dictionary<Ticker, List<TechnicalIndicator>>
            {
                [_ticker] = BuildIndicators(
                    ("RSI14", 25), ("MACD", -1), ("EMA12", 5), ("SMA20", 6), ("ADX14", 10)),
            },
            FundamentalScores = new Dictionary<Ticker, int> { [_ticker] = 2 },
        };

        var step = new GenerateSignalsStep(_storage.Object);
        await step.ExecuteAsync(context);

        _signalRepo.Verify(
            x => x.AddAsync(It.Is<TradeSignal>(s =>
            s.Action == TradeAction.Buy && s.Confidence == ConfidenceLevel.Low)), Times.Once);
    }

    /// <summary>
    ///     Тест 3: Buy (Medium) при RSI меньше 30 и слабом score
    /// </summary>
    [Fact(DisplayName = "Тест 3: Buy (Medium) при RSI < 30 и слабом score")]
    public async Task ExecuteAsync_ShouldGenerateBuyMediumSignal()
    {
        var context = new PortfolioAnalysisContext
        {
            Indicators = new Dictionary<Ticker, List<TechnicalIndicator>>
            {
                [_ticker] = BuildIndicators(
                    ("RSI14", 25), ("MACD", 1), ("EMA12", 7), ("SMA20", 6), ("ADX14", 25)),
            },
            FundamentalScores = new Dictionary<Ticker, int> { [_ticker] = 3 },
        };

        var step = new GenerateSignalsStep(_storage.Object);
        await step.ExecuteAsync(context);

        _signalRepo.Verify(
            x => x.AddAsync(It.Is<TradeSignal>(s =>
            s.Action == TradeAction.Buy && s.Confidence == ConfidenceLevel.Medium)), Times.Once);
    }

    /// <summary>
    ///     Тест 4: Buy (High) при RSI меньше 30 и сильном score
    /// </summary>
    [Fact(DisplayName = "Тест 4: Buy (High) при RSI < 30 и сильном score")]
    public async Task ExecuteAsync_ShouldGenerateBuyHighSignal()
    {
        var context = new PortfolioAnalysisContext
        {
            Indicators = new Dictionary<Ticker, List<TechnicalIndicator>>
            {
                [_ticker] = BuildIndicators(
                    ("RSI14", 25), ("MACD", 1), ("EMA12", 7), ("SMA20", 6), ("ADX14", 25)),
            },
            FundamentalScores = new Dictionary<Ticker, int> { [_ticker] = 5 },
        };

        var step = new GenerateSignalsStep(_storage.Object);
        await step.ExecuteAsync(context);

        _signalRepo.Verify(
            x => x.AddAsync(It.Is<TradeSignal>(s =>
            s.Action == TradeAction.Buy && s.Confidence == ConfidenceLevel.High)), Times.Once);
    }

    /// <summary>
    ///     Тест 5: Sell (Medium) при RSI > 70 без подтверждений
    /// </summary>
    [Fact(DisplayName = "Тест 5: Sell (Medium) при RSI > 70 без подтверждений")]
    public async Task ExecuteAsync_ShouldGenerateSellMediumSignal()
    {
        var context = new PortfolioAnalysisContext
        {
            Indicators = new Dictionary<Ticker, List<TechnicalIndicator>>
            {
                [_ticker] = BuildIndicators(
                    ("RSI14", 75), ("MACD", 1), ("EMA12", 10), ("SMA20", 9), ("ADX14", 10)),
            },
            FundamentalScores = new Dictionary<Ticker, int> { [_ticker] = 2 },
        };

        var step = new GenerateSignalsStep(_storage.Object);
        await step.ExecuteAsync(context);

        _signalRepo.Verify(
            x => x.AddAsync(It.Is<TradeSignal>(s =>
            s.Action == TradeAction.Sell && s.Confidence == ConfidenceLevel.Medium)), Times.Once);
    }

    /// <summary>
    ///     Тест 6: Sell (High) при RSI > 70 и подтверждении
    /// </summary>
    [Fact(DisplayName = "Тест 6: Sell (High) при RSI > 70 и подтверждении")]
    public async Task ExecuteAsync_ShouldGenerateSellHighSignal()
    {
        var context = new PortfolioAnalysisContext
        {
            Indicators = new Dictionary<Ticker, List<TechnicalIndicator>>
            {
                [_ticker] = BuildIndicators(
                    ("RSI14", 75), ("MACD", -1), ("EMA12", 10), ("SMA20", 9), ("ADX14", 30)),
            },
            FundamentalScores = new Dictionary<Ticker, int> { [_ticker] = 4 },
        };

        var step = new GenerateSignalsStep(_storage.Object);
        await step.ExecuteAsync(context);

        _signalRepo.Verify(
            x => x.AddAsync(It.Is<TradeSignal>(s =>
            s.Action == TradeAction.Sell && s.Confidence == ConfidenceLevel.High)), Times.Once);
    }

    /// <summary>
    ///     Тест 7: Hold при RSI между 30 и 70
    /// </summary>
    [Fact(DisplayName = "Тест 7: Hold при RSI между 30 и 70")]
    public async Task ExecuteAsync_ShouldGenerateHoldSignal()
    {
        var context = new PortfolioAnalysisContext
        {
            Indicators = new Dictionary<Ticker, List<TechnicalIndicator>>
            {
                [_ticker] = BuildIndicators(
                    ("RSI14", 50), ("MACD", 0), ("EMA12", 5), ("SMA20", 5), ("ADX14", 10)),
            },
            FundamentalScores = new Dictionary<Ticker, int> { [_ticker] = 3 },
        };

        var step = new GenerateSignalsStep(_storage.Object);
        await step.ExecuteAsync(context);

        _signalRepo.Verify(
            x => x.AddAsync(It.Is<TradeSignal>(s =>
            s.Action == TradeAction.Hold)), Times.Once);
    }

    private static TechnicalIndicator Indicator(string name, double value, int offset = 0)
    {
        return new TechnicalIndicator
        {
            Name = name,
            Value = value,
            Date = DateTime.UtcNow.AddMinutes(-offset),
        };
    }

    private static List<TechnicalIndicator> BuildIndicators(params (string Name, double Value)[] values)
    {
        return values.Select(v => Indicator(v.Name, v.Value)).ToList();
    }
}
