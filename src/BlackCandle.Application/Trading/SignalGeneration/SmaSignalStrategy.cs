using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Trading.SignalGeneration;

/// <summary>
///     Стратегия сигналов на SMA
/// </summary>
internal sealed class SmaSignalStrategy : ISignalGenerationStrategy
{
    /// <inheritdoc/>
    public TechnicalScore? GenerateScore(Ticker ticker, List<TechnicalIndicator> indicators)
    {
        var sma = indicators
            .Where(i => i.Name == "SMA20")
            .OrderByDescending(i => i.Date)
            .FirstOrDefault();

        var price = indicators
            .Where(i => i.Name == "CLOSE")
            .OrderByDescending(i => i.Date)
            .FirstOrDefault();

        if (sma is not { Value: not null } || price is not { Value: not null })
        {
            return null;
        }

        TradeAction action;
        if (price.Value > sma.Value)
        {
            action = TradeAction.Buy;
        }
        else
        {
            if (price.Value < sma.Value)
            {
                action = TradeAction.Sell;
            }
            else
            {
                action = TradeAction.Hold;
            }
        }

        return new TechnicalScore(action, ConfidenceLevel.Medium)
        {
            IndicatorName = "SMA20",
            Value = sma.Value.Value,
            Reason = $"Цена {price.Value.Value:F2} против SMA {sma.Value.Value:F2}",
        };
    }
}
