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
    public string Ticker { get; set; } = string.Empty;

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
}