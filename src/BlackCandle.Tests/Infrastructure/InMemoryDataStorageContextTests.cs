using BlackCandle.Domain.Entities;
using BlackCandle.Infrastructure.Persistence.InMemory;

namespace BlackCandle.Tests.Infrastructure;

/// <summary>
///     Тесты на <see href="InMemoryDataStorageContext" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item><description>Проверка, что все репозитории не null</description></item>
///         <item><description>Проверка, что каждый репозиторий реализован через InMemoryRepository</description></item>
///     </list>
/// </remarks>
public sealed class InMemoryDataStorageContextTests
{
    /// <summary>
    ///     Тест 1: Проверка, что все репозитории не null
    /// </summary>
    [Fact(DisplayName = "Тест 1: Проверка, что все репозитории не null")]
    public void AllRepositories_ShouldNotBeNull()
    {
        // Arrange
        var context = new InMemoryDataStorageContext();

        // Assert
        Assert.NotNull(context.PortfolioAssets);
        Assert.NotNull(context.TradeSignals);
        Assert.NotNull(context.Fundamentals);
        Assert.NotNull(context.AnalysisResults);
        Assert.NotNull(context.ExecutedTrades);
        Assert.NotNull(context.Logs);
        Assert.NotNull(context.BotSettings);
        Assert.NotNull(context.Marketdata);
    }

    /// <summary>
    ///     Тест 2: Проверка, что все репозитории — InMemoryRepository
    /// </summary>
    [Fact(DisplayName = "Тест 2: Проверка, что все репозитории — InMemoryRepository")]
    public void AllRepositories_ShouldBeOfType_InMemoryRepository()
    {
        // Arrange
        var context = new InMemoryDataStorageContext();

        // Assert
        Assert.IsType<InMemoryRepository<PortfolioAsset>>(context.PortfolioAssets);
        Assert.IsType<InMemoryRepository<TradeSignal>>(context.TradeSignals);
        Assert.IsType<InMemoryRepository<FundamentalData>>(context.Fundamentals);
        Assert.IsType<InMemoryRepository<PortfolioAnalysisResult>>(context.AnalysisResults);
        Assert.IsType<InMemoryRepository<ExecutedTrade>>(context.ExecutedTrades);
        Assert.IsType<InMemoryRepository<LogEntry>>(context.Logs);
        Assert.IsType<InMemoryRepository<BotSettings>>(context.BotSettings);
        Assert.IsType<InMemoryRepository<PriceHistoryPoint>>(context.Marketdata);
    }
}
