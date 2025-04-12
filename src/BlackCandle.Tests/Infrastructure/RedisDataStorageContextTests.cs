using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Infrastructure.Persistence.Redis;
using BlackCandle.Infrastructure.Persistence.Redis.Entities;

using FluentAssertions;

using Microsoft.Extensions.Options;

using Moq;

using StackExchange.Redis;

namespace BlackCandle.Tests.Infrastructure;

/// <summary>
///     Тесты для <see cref="RedisDataStorageContext" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Все репозитории создаются</item>
///         <item>Каждое свойство возвращает соответствующий тип</item>
///     </list>
/// </remarks>
public sealed class RedisDataStorageContextTests
{
    /// <summary>
    ///     Тест 1: Контекст создаёт все репозитории
    /// </summary>
    [Fact(DisplayName = "Тест 1: Контекст создаёт все репозитории")]
    public void Context_ShouldInitializeAllRepositories()
    {
        // Arrange
        var redis = new Mock<IConnectionMultiplexer>().Object;
        var options = new RedisOptions { Prefix = "test:" };
        var mockOptions = new Mock<IOptions<RedisOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);

        // Act
        var context = new RedisDataStorageContext(redis, mockOptions.Object);

        // Assert
        context.PortfolioAssets.Should().BeOfType<RedisRepository<PortfolioAsset, RedisPortfolioAsset>>();
        context.TradeSignals.Should().BeOfType<RedisRepository<TradeSignal, RedisTradeSignal>>();
        context.Fundamentals.Should().BeOfType<RedisRepository<FundamentalData, RedisFundamentalData>>();
        context.AnalysisResults.Should().BeOfType<RedisRepository<PortfolioAnalysisResult, RedisPortfolioAnalysisResult>>();
        context.ExecutedTrades.Should().BeOfType<RedisRepository<ExecutedTrade, RedisExecutedTrade>>();
        context.Logs.Should().BeOfType<RedisRepository<LogEntry, RedisLogEntry>>();
        context.BotSettings.Should().BeOfType<RedisRepository<BotSettings, RedisBotSettings>>();
        context.Marketdata.Should().BeOfType<RedisRepository<PriceHistoryPoint, RedisPriceHistoryPoint>>();
        context.PipelineRuns.Should().BeOfType<RedisRepository<PipelineExecutionRecord, RedisPipelineExecutionRecord>>();
    }
}
