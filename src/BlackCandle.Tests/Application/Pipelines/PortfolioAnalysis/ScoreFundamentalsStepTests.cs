using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="ScoreFundamentalsStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Игнорируется актив без фундаменталки</item>
///         <item>+1 за PERatio меньше 15</item>
///         <item>+1 за PBRatio меньше 3</item>
///         <item>+1 за DividendYield > 4</item>
///         <item>+1 за ROE > 10</item>
///         <item>+1 за MarketCap > 100_000</item>
///         <item>Полный скор = 5 при всех выполненных условиях</item>
///     </list>
/// </remarks>
public sealed class ScoreFundamentalsStepTests
{
    private readonly Mock<IRepository<PortfolioAsset>> _portfolioRepo = new();
    private readonly Mock<IRepository<FundamentalData>> _fundamentalRepo = new();
    private readonly Mock<IDataStorageContext> _dataStorage = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ScoreFundamentalsStepTests"/> class.
    /// </summary>
    public ScoreFundamentalsStepTests()
    {
        _dataStorage.Setup(x => x.PortfolioAssets).Returns(_portfolioRepo.Object);
        _dataStorage.Setup(x => x.Fundamentals).Returns(_fundamentalRepo.Object);
    }

    /// <summary>
    ///     Тест 1: Игнорируется актив без фундаменталки
    /// </summary>
    [Fact(DisplayName = "Тест 1: Игнорируется актив без фундаменталки")]
    public async Task ExecuteAsync_ShouldSkipAsset_IfNoFundamental()
    {
        // Arrange
        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync([Asset("AAPL")]);
        _fundamentalRepo.Setup(x => x.GetByIdAsync("AAPL")).ReturnsAsync((FundamentalData?)null);

        var context = new PortfolioAnalysisContext();
        var step = new ScoreFundamentalsStep(_dataStorage.Object);

        // Act
        await step.ExecuteAsync(context);

        // Assert
        Assert.Empty(context.FundamentalScores);
    }

    /// <summary>
    ///     Тест 2: +1 за PERatio меньше 15
    /// </summary>
    [Fact(DisplayName = "Тест 2: +1 за PERatio < 15")]
    public async Task ExecuteAsync_ShouldAddScore_ForPERatio()
    {
        // Arrange
        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync([Asset("AAPL")]);
        _fundamentalRepo.Setup(x => x.GetByIdAsync("AAPL")).ReturnsAsync(Fund(10));

        var context = new PortfolioAnalysisContext();
        var step = new ScoreFundamentalsStep(_dataStorage.Object);

        // Act
        await step.ExecuteAsync(context);

        // Assert
        Assert.Equal(1, context.FundamentalScores[new Ticker { Symbol = "AAPL" }]);
    }

    /// <summary>
    ///     Тест 3: +1 за PBRatio меньше 3
    /// </summary>
    [Fact(DisplayName = "Тест 3: +1 за PBRatio < 3")]
    public async Task ExecuteAsync_ShouldAddScore_ForPBRatio()
    {
        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync([Asset("AAPL")]);
        _fundamentalRepo.Setup(x => x.GetByIdAsync("AAPL")).ReturnsAsync(Fund(pb: 2));

        var context = new PortfolioAnalysisContext();
        var step = new ScoreFundamentalsStep(_dataStorage.Object);

        await step.ExecuteAsync(context);

        Assert.Equal(1, context.FundamentalScores[new Ticker { Symbol = "AAPL" }]);
    }

    /// <summary>
    ///     Тест 4: +1 за DividendYield > 4
    /// </summary>
    [Fact(DisplayName = "Тест 4: +1 за DividendYield > 4")]
    public async Task ExecuteAsync_ShouldAddScore_ForDividendYield()
    {
        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync([Asset("AAPL")]);
        _fundamentalRepo.Setup(x => x.GetByIdAsync("AAPL")).ReturnsAsync(Fund(div: 5));

        var context = new PortfolioAnalysisContext();
        var step = new ScoreFundamentalsStep(_dataStorage.Object);

        await step.ExecuteAsync(context);

        Assert.Equal(1, context.FundamentalScores[new Ticker { Symbol = "AAPL" }]);
    }

    /// <summary>
    ///     Тест 5: +1 за ROE > 10
    /// </summary>
    [Fact(DisplayName = "Тест 5: +1 за ROE > 10")]
    public async Task ExecuteAsync_ShouldAddScore_ForROE()
    {
        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync([Asset("AAPL")]);
        _fundamentalRepo.Setup(x => x.GetByIdAsync("AAPL")).ReturnsAsync(Fund(roe: 11));

        var context = new PortfolioAnalysisContext();
        var step = new ScoreFundamentalsStep(_dataStorage.Object);

        await step.ExecuteAsync(context);

        Assert.Equal(1, context.FundamentalScores[new Ticker { Symbol = "AAPL" }]);
    }

    /// <summary>
    ///     Тест 6: +1 за MarketCap > 100_000
    /// </summary>
    [Fact(DisplayName = "Тест 6: +1 за MarketCap > 100_000")]
    public async Task ExecuteAsync_ShouldAddScore_ForMarketCap()
    {
        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync([Asset("AAPL")]);
        _fundamentalRepo.Setup(x => x.GetByIdAsync("AAPL")).ReturnsAsync(Fund(cap: 150_000));

        var context = new PortfolioAnalysisContext();
        var step = new ScoreFundamentalsStep(_dataStorage.Object);

        await step.ExecuteAsync(context);

        Assert.Equal(1, context.FundamentalScores[new Ticker { Symbol = "AAPL" }]);
    }

    /// <summary>
    ///     Тест 7: Максимальный скор = 5
    /// </summary>
    [Fact(DisplayName = "Тест 7: Максимальный скор = 5")]
    public async Task ExecuteAsync_ShouldScoreAllCriteria()
    {
        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync([Asset("AAPL")]);
        _fundamentalRepo.Setup(x => x.GetByIdAsync("AAPL")).ReturnsAsync(Fund(10, 2, 6, 15, 200_000));

        var context = new PortfolioAnalysisContext();
        var step = new ScoreFundamentalsStep(_dataStorage.Object);

        await step.ExecuteAsync(context);

        Assert.Equal(5, context.FundamentalScores[new Ticker { Symbol = "AAPL" }]);
    }

    private static PortfolioAsset Asset(string symbol)
    {
        return new PortfolioAsset { Ticker = new Ticker { Symbol = symbol } };
    }

    private static FundamentalData Fund(decimal? pe = null, decimal? pb = null, decimal? div = null,
        decimal? roe = null, decimal? cap = null)
    {
        return new FundamentalData
        {
            Ticker = "AAPL",
            PERatio = pe,
            PBRatio = pb,
            DividendYield = div,
            ROE = roe,
            MarketCap = cap,
        };
    }
}
