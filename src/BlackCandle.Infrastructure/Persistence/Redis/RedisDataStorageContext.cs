using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Infrastructure.Persistence.Redis.Entities;

using Microsoft.Extensions.Options;

using StackExchange.Redis;

namespace BlackCandle.Infrastructure.Persistence.Redis;

/// <summary>
///     Контекст хранилища на основе Redis
/// </summary>
internal sealed class RedisDataStorageContext(IConnectionMultiplexer redis, IOptions<RedisOptions> options) : IDataStorageContext
{
    /// <inheritdoc />
    public IRepository<PortfolioAsset> PortfolioAssets { get; } = new RedisRepository<PortfolioAsset, RedisPortfolioAsset>(redis, options.Value);

    /// <inheritdoc />
    public IRepository<TradeSignal> TradeSignals { get; } = new RedisRepository<TradeSignal, RedisTradeSignal>(redis, options.Value);

    /// <inheritdoc />
    public IRepository<FundamentalData> Fundamentals { get; } = new RedisRepository<FundamentalData, RedisFundamentalData>(redis, options.Value);

    /// <inheritdoc />
    public IRepository<PortfolioAnalysisResult> AnalysisResults { get; } = new RedisRepository<PortfolioAnalysisResult, RedisPortfolioAnalysisResult>(redis, options.Value);

    /// <inheritdoc />
    public IRepository<ExecutedTrade> ExecutedTrades { get; } = new RedisRepository<ExecutedTrade, RedisExecutedTrade>(redis, options.Value);

    /// <inheritdoc />
    public IRepository<LogEntry> Logs { get; } = new RedisRepository<LogEntry, RedisLogEntry>(redis, options.Value);

    /// <inheritdoc />
    public IRepository<BotSettings> BotSettings { get; } = new RedisRepository<BotSettings, RedisBotSettings>(redis, options.Value);

    /// <inheritdoc />
    public IRepository<PriceHistoryPoint> Marketdata { get; } = new RedisRepository<PriceHistoryPoint, RedisPriceHistoryPoint>(redis, options.Value);

    /// <inheritdoc />
    public IRepository<PipelineExecutionRecord> PipelineRuns { get; } = new RedisRepository<PipelineExecutionRecord, RedisPipelineExecutionRecord>(redis, options.Value);
}
