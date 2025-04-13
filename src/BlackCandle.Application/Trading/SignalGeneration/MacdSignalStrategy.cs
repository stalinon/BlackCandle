using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Trading.SignalGeneration;

/// <summary>
///     Стратегия на MACD
/// </summary>
internal sealed class MacdSignalStrategy : ISignalGenerationStrategy
{
    /// <inheritdoc/>
    public TechnicalScore? GenerateScore(Ticker ticker, List<TechnicalIndicator> indicators)
    {
        var macd = indicators
            .Where(i => i.Name == "MACD")
            .OrderByDescending(i => i.Date)
            .FirstOrDefault();

        if (macd is not { Value: not null })
        {
            return null;
        }

        var action = macd.Value switch
        {
            > 0 => TradeAction.Buy,
            < 0 => TradeAction.Sell,
            _ => TradeAction.Hold,
        };

        return new TechnicalScore(action, ConfidenceLevel.Medium)
        {
            IndicatorName = "MACD",
            Value = macd.Value.Value,
            Reason = $"MACD = {macd.Value.Value:F2}",
        };
    }
}
