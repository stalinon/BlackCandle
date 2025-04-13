using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Trading.SignalGeneration;

/// <summary>
///     Стратегия сигналов на основе RSI
/// </summary>
internal sealed class RsiSignalStrategy : ISignalGenerationStrategy
{
    /// <inheritdoc/>
    public TechnicalScore? GenerateScore(Ticker ticker, List<TechnicalIndicator> indicators)
    {
        var rsi = indicators
            .Where(i => i.Name == "RSI14")
            .OrderByDescending(i => i.Date)
            .FirstOrDefault();

        if (rsi is not { Value: not null })
        {
            return null;
        }

        var score = rsi.Value switch
        {
            < 30 => new TechnicalScore(TradeAction.Buy, ConfidenceLevel.Medium)
            {
                Reason = "RSI < 30 (перепроданность)",
            },
            > 70 => new TechnicalScore(TradeAction.Sell, ConfidenceLevel.Medium)
            {
                Reason = "RSI > 70 (перекупленность)",
            },
            _ => new TechnicalScore(TradeAction.Hold, ConfidenceLevel.Medium)
            {
                Reason = "RSI в норме",
            },
        };

        score.IndicatorName = "RSI14";
        score.Value = rsi.Value.Value;

        return score;
    }
}
