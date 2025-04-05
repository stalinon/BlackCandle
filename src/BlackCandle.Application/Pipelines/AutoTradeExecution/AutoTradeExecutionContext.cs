using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Контекст пайплайна автотрейда
/// </summary>
public class AutoTradeExecutionContext
{
    /// <summary>
    ///     Дата исполнения заявок
    /// </summary>
    public DateTime ExecutionTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     Сигналы
    /// </summary>
    public List<TradeSignal> Signals { get; set; } = [];

    /// <summary>
    ///     Совершенные сделки
    /// </summary>
    public List<ExecutedTrade> ExecutedTrades { get; set; } = [];
}
