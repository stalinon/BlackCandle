using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces;

/// <summary>
///     Контекст хранилища данных, агрегирующий все репозитории
/// </summary>
public interface IDataStorageContext
{
    /// <summary>
    ///     Репозиторий портфеля
    /// </summary>
    IRepository<PortfolioAsset> PortfolioAssets { get; }

    /// <summary>
    ///     Репозиторий торговых сигналов
    /// </summary>
    IRepository<TradeSignal> TradeSignals { get; }

    /// <summary>
    ///     Репозиторий фундаментальных метрик
    /// </summary>
    IRepository<FundamentalData> Fundamentals { get; }

    /// <summary>
    ///     Репозиторий результатов анализа
    /// </summary>
    IRepository<PortfolioAnalysisResult> AnalysisResults { get; }

    /// <summary>
    ///     Репозиторий исполненных сделок
    /// </summary>
    IRepository<ExecutedTrade> ExecutedTrades { get; }

    /// <summary>
    ///     Репозиторий логов
    /// </summary>
    IRepository<LogEntry> Logs { get; }

    /// <summary>
    ///     Репозиторий настроек бота
    /// </summary>
    IRepository<BotSettings> BotSettings { get; }
    
    /// <summary>
    ///     Репозиторий исторических данных
    /// </summary>
    IRepository<PriceHistoryPoint> Marketdata { get; }
}