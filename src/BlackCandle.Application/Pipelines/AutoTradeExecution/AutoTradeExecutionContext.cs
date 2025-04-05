using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

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

    #region Preview

    /// <summary>
    ///     Режим превью (не исполняются заявки)
    /// </summary>
    public bool PreviewMode { get; set; } = false;

    /// <summary>
    ///     Сделки, которые могли бы использоваться
    /// </summary>
    public List<OrderPreview> PreviewOrders { get; set; } = new();

    #endregion
}
