using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Trading.SignalGeneration;

/// <summary>
///     Стратегия на ADX
/// </summary>
internal sealed class AdxSignalStrategy : ISignalGenerationStrategy
{
    /// <inheritdoc/>
    public TechnicalScore? GenerateScore(Ticker ticker, List<TechnicalIndicator> indicators)
    {
        var adx = indicators
            .Where(i => i.Name == "ADX14")
            .OrderByDescending(i => i.Date)
            .FirstOrDefault();

        if (adx is not { Value: not null })
        {
            return null;
        }

        var value = adx.Value.Value;

        var action = value switch
        {
            > 25 => TradeAction.Buy,
            < 20 => TradeAction.Hold,
            _ => TradeAction.Hold,
        };

        return new TechnicalScore(action, ConfidenceLevel.Low)
        {
            IndicatorName = "ADX14",
            Value = value,
            Reason = $"ADX = {value:F2}",
        };
    }
}
