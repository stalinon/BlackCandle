using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;

namespace BlackCandle.Infrastructure.Persistence.InMemory;

/// <summary>
///     Контекст хранилища на основе памяти (in-memory)
/// </summary>
internal sealed class InMemoryDataStorageContext : IDataStorageContext
{
    /// <inheritdoc />
    public IRepository<PortfolioAsset> PortfolioAssets { get; } = new InMemoryRepository<PortfolioAsset>();

    /// <inheritdoc />
    public IRepository<TradeSignal> TradeSignals { get; } = new InMemoryRepository<TradeSignal>();

    /// <inheritdoc />
    public IRepository<FundamentalData> Fundamentals { get; } = new InMemoryRepository<FundamentalData>();

    /// <inheritdoc />
    public IRepository<PortfolioAnalysisResult> AnalysisResults { get; } =
        new InMemoryRepository<PortfolioAnalysisResult>();

    /// <inheritdoc />
    public IRepository<ExecutedTrade> ExecutedTrades { get; } = new InMemoryRepository<ExecutedTrade>();

    /// <inheritdoc />
    public IRepository<LogEntry> Logs { get; } = new InMemoryRepository<LogEntry>();

    /// <inheritdoc />
    public IRepository<BotSettings> BotSettings { get; } = new InMemoryRepository<BotSettings>();

    /// <inheritdoc />
    public IRepository<PriceHistoryPoint> Marketdata { get; } = new InMemoryRepository<PriceHistoryPoint>();

    /// <inheritdoc cref="IDataStorageContext.PipelineRuns"/>
    public IRepository<PipelineExecutionRecord> PipelineRuns { get; } = new InMemoryRepository<PipelineExecutionRecord>();
}
