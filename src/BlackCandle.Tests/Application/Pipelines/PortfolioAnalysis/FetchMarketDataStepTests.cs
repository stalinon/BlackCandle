using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="FetchMarketDataStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>MarketData запрашивается для каждого актива</item>
///         <item>MarketData очищается перед добавлением</item>
///         <item>MarketData сохраняется</item>
///         <item>Фундаменталка сохраняется, если она есть</item>
///         <item>Null-фундаменталка не сохраняется</item>
///     </list>
/// </remarks>
public sealed class FetchMarketDataStepTests
{
    private readonly Ticker _ticker1 = new() { Symbol = "AAPL" };
    private readonly Ticker _ticker2 = new() { Symbol = "SBER" };

    private readonly Mock<IMarketDataClient> _marketMock = new();
    private readonly Mock<IFundamentalDataClient> _fundamentalsMock = new();
    private readonly Mock<IInvestApiFacade> _investMock = new();
    private readonly Mock<IRepository<PortfolioAsset>> _portfolioRepo = new();
    private readonly Mock<IRepository<PriceHistoryPoint>> _marketRepo = new();
    private readonly Mock<IRepository<FundamentalData>> _fundamentalsRepo = new();
    private readonly Mock<IDataStorageContext> _storageMock = new();

    private readonly FetchMarketDataStep _step;
    private readonly PortfolioAnalysisContext _context = new();

    /// <inheritdoc cref="FetchMarketDataStepTests"/>
    public FetchMarketDataStepTests()
    {
        List<PortfolioAsset> assets =
        [
            new() { Ticker = _ticker1 },
            new() { Ticker = _ticker2 }
        ];

        _context.Tickers.Add(_ticker1);
        _context.Tickers.Add(_ticker2);

        _marketMock
            .Setup(x => x.GetHistoricalDataAsync(It.IsAny<Ticker>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync([new PriceHistoryPoint { Ticker = _ticker1, Open = 100 }]);

        _fundamentalsMock.Setup(x => x.GetFundamentalsAsync(_ticker1))
            .ReturnsAsync(new FundamentalData { Ticker = "AAPL" });

        _fundamentalsMock.Setup(x => x.GetFundamentalsAsync(_ticker2))
            .ReturnsAsync((FundamentalData?)null);

        _investMock.Setup(x => x.Marketdata).Returns(_marketMock.Object);
        _investMock.Setup(x => x.Fundamentals).Returns(_fundamentalsMock.Object);

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync(assets);

        _storageMock.Setup(x => x.PortfolioAssets).Returns(_portfolioRepo.Object);
        _storageMock.Setup(x => x.Marketdata).Returns(_marketRepo.Object);
        _storageMock.Setup(x => x.Fundamentals).Returns(_fundamentalsRepo.Object);

        _step = new FetchMarketDataStep(_investMock.Object, _storageMock.Object);
    }

    /// <summary>
    ///     Тест 1: MarketData запрашивается для каждого актива
    /// </summary>
    [Fact(DisplayName = "Тест 1: MarketData запрашивается для каждого актива")]
    public async Task ExecuteAsync_ShouldCallMarketDataApi_ForEachAsset()
    {
        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _marketMock.Verify(
            x => x.GetHistoricalDataAsync(It.IsAny<Ticker>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Exactly(2));
    }

    /// <summary>
    ///     Тест 2: MarketData очищается перед добавлением
    /// </summary>
    [Fact(DisplayName = "Тест 2: MarketData очищается перед добавлением")]
    public async Task ExecuteAsync_ShouldTruncateMarketData_BeforeInsert()
    {
        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _marketRepo.Verify(x => x.TruncateAsync(), Times.Exactly(1));
    }

    /// <summary>
    ///     Тест 3: MarketData сохраняется
    /// </summary>
    [Fact(DisplayName = "Тест 3: MarketData сохраняется")]
    public async Task ExecuteAsync_ShouldStoreMarketData()
    {
        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _marketRepo.Verify(x => x.AddRangeAsync(It.IsAny<IReadOnlyCollection<PriceHistoryPoint>>()), Times.Exactly(2));
    }

    /// <summary>
    ///     Тест 4: Фундаменталка сохраняется, если она есть
    /// </summary>
    [Fact(DisplayName = "Тест 4: Фундаменталка сохраняется, если она есть")]
    public async Task ExecuteAsync_ShouldStoreFundamentals_IfNotNull()
    {
        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _fundamentalsRepo.Verify(x => x.AddAsync(It.Is<FundamentalData>(f => f.Ticker == "AAPL")), Times.Once);
    }

    /// <summary>
    ///     Тест 5: Null-фундаменталка не сохраняется
    /// </summary>
    [Fact(DisplayName = "Тест 5: Null-фундаменталка не сохраняется")]
    public async Task ExecuteAsync_ShouldNotSaveNullFundamentals()
    {
        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _fundamentalsRepo.Verify(x => x.AddAsync(It.Is<FundamentalData>(f => f.Ticker == "SBER")), Times.Never);
    }
}
