using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Trading.SignalGeneration;

/// <inheritdoc cref="ISignalGenerationStrategy" />
internal sealed class DefaultSignalGenerationStrategy : ISignalGenerationStrategy
{
    /// <inheritdoc/>
    public TradeSignal? Generate(Ticker ticker, List<TechnicalIndicator> indicators, int score, DateTime now)
    {
        var rsi = GetLatest(indicators, "RSI14");
        var ema = GetLatest(indicators, "EMA12");
        var sma = GetLatest(indicators, "SMA20");
        var adx = GetLatest(indicators, "ADX14");
        var macd = GetLatest(indicators, "MACD");

        if (rsi is null || ema is null || sma is null || adx is null || macd is null)
        {
            return null;
        }

        var action = TradeAction.Hold;
        var confidence = ConfidenceLevel.Low;
        var reasons = new List<string>
        {
            $"RSI={rsi.Value:F2}",
            $"MACD={macd.Value:F2}",
            $"EMA={ema.Value:F2}",
            $"SMA={sma.Value:F2}",
            $"ADX={adx.Value:F2}",
            $"Score={score}",
        };

        // Упрощенные условия:
        if (rsi.Value < 40 && ema.Value > sma.Value)
        {
            action = TradeAction.Buy;
            confidence = score >= 3 ? ConfidenceLevel.Medium : ConfidenceLevel.Low;
        }
        else if (rsi.Value > 60 && ema.Value < sma.Value)
        {
            action = TradeAction.Sell;
            confidence = score >= 3 ? ConfidenceLevel.Medium : ConfidenceLevel.Low;
        }

        return new TradeSignal
        {
            Ticker = ticker,
            Action = action,
            Confidence = confidence,
            Reason = string.Join(" | ", reasons),
            Date = now,
            Score = score,
        };
    }

    private static TechnicalIndicator? GetLatest(List<TechnicalIndicator> indicators, string name)
    {
        return indicators
            .Where(i => i.Name == name && i.Value.HasValue)
            .OrderByDescending(i => i.Date)
            .FirstOrDefault();
    }
}
