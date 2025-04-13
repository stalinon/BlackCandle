using BlackCandle.Domain.Enums;

namespace BlackCandle.Domain.ValueObjects;

/// <summary>
///     Техническая оценка
/// </summary>
public class TechnicalScore
{
    /// <summary>
    ///     Конструктор через уверенность и направление
    /// </summary>
    public TechnicalScore(TradeAction action, ConfidenceLevel confidence)
    {
        var sign = action switch
        {
            TradeAction.Buy => 1,
            TradeAction.Sell => -1,
            _ => 0,
        };

        var score = sign * confidence switch
        {
            ConfidenceLevel.Low => 1,
            ConfidenceLevel.Medium => 2,
            ConfidenceLevel.High => 3,
            _ => 0,
        };

        Score = score;
    }

    /// <summary>
    ///     Дефолтный конструктор
    /// </summary>
    public TechnicalScore()
    { }

    /// <summary>
    ///     Название индикатора
    /// </summary>
    public string IndicatorName { get; set; } = string.Empty;

    /// <summary>
    ///     Значение индикатора
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    ///     Скор (отрицательный - продажа, положительный - покупка)
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    ///     Причина сигнала
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    ///     Скопировать
    /// </summary>
    public TechnicalScore Copy() => (TechnicalScore)MemberwiseClone();
}
