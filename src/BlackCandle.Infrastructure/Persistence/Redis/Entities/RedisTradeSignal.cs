using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Infrastructure.Persistence.Redis.Entities;

/// <summary>
///     Торговый сигнал для Redis
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal sealed class RedisTradeSignal : IStorageEntity<TradeSignal>
{
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
    ///     Технические оценки
    /// </summary>
    public List<TechnicalScore> TechnicalScores { get; set; } = new();

    /// <inheritdoc />
    public TradeSignal ToEntity() => new()
    {
        Ticker = Ticker,
        Action = Action,
        Reason = Reason,
        Confidence = Confidence,
        Date = Date,
        FundamentalScore = FundamentalScore,
        TechnicalScores = TechnicalScores.Select(t => t.Copy()).ToList(),
    };

    /// <inheritdoc />
    public IStorageEntity<TradeSignal> ToStorageEntity(TradeSignal entity)
    {
        return new RedisTradeSignal
        {
            Ticker = entity.Ticker,
            Action = entity.Action,
            Reason = entity.Reason,
            Confidence = entity.Confidence,
            Date = entity.Date,
            FundamentalScore = entity.FundamentalScore,
            TechnicalScores = entity.TechnicalScores.Select(t => t.Copy()).ToList(),
        };
    }
}
