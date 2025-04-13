using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;
using BlackCandle.Domain.ValueObjects;

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
    public Ticker Ticker { get; set; } = null!;

    /// <summary>
    ///     Дата генерации
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    ///     Действие
    /// </summary>
    public TradeAction Action { get; set; }

    /// <summary>
    ///     Причина сигнала
    /// </summary>
    public string Reason { get; set; } = default!;

    /// <summary>
    ///     Уверенность
    /// </summary>
    public ConfidenceLevel Confidence { get; set; }

    /// <summary>
    ///     Результативный балл сигнала (по фундаменталке)
    /// </summary>
    public int FundamentalScore { get; set; }

    /// <summary>
    ///     Возможные деньги на сигнал
    /// </summary>
    public decimal? AllocatedCash { get; set; }

    /// <summary>
    ///     Технические оценки
    /// </summary>
    public List<TechnicalScore> TechnicalScores { get; set; } = new();
}
