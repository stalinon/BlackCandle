using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Trading.SignalGeneration;

/// <summary>
///     Стратегия сигналов на EMA
/// </summary>
internal sealed class EmaSignalStrategy : ISignalGenerationStrategy
{
    /// <inheritdoc/>
    public TechnicalScore? GenerateScore(Ticker ticker, List<TechnicalIndicator> indicators)
    {
        var ema = indicators
            .Where(i => i.Name == "EMA12")
            .OrderByDescending(i => i.Date)
            .FirstOrDefault();

        var price = indicators
            .Where(i => i.Name == "CLOSE")
            .OrderByDescending(i => i.Date)
            .FirstOrDefault();

        if (ema is not { Value: not null } || price is not { Value: not null })
        {
            return null;
        }

        TradeAction action;
        if (price.Value > ema.Value)
        {
            action = TradeAction.Buy;
        }
        else
        {
            if (price.Value < ema.Value)
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
            IndicatorName = "EMA12",
            Value = ema.Value.Value,
            Reason = $"Цена {price.Value.Value:F2} против EMA {ema.Value.Value:F2}",
        };
    }
}
