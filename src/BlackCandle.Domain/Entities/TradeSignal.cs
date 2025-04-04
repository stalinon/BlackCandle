using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Domain.Entities;

/// <summary>
///     Торговый сигнал
/// </summary>
public class TradeSignal : IEntity
{
    /// <inheritdoc />
    public string Id => $"{Ticker}_{Date:yyyyMMdd}";

    /// <summary>
    ///     Символ
    /// </summary>
    public Ticker Ticker { get; set; }

    /// <summary>
    ///     Действие
    /// </summary>
    public TradeAction Action { get; set; }

    /// <summary>
    ///     Причина сигнала
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    ///     Уверенность
    /// </summary>
    public ConfidenceLevel Confidence { get; set; }

    /// <summary>
    ///     Дата генерации
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    ///     Результативный балл сигнала (по фундаменталке)
    /// </summary>
    public int Score { get; set; }
}