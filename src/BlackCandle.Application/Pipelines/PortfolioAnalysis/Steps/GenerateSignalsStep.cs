using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Генерация сигналов на основе тех. индикаторов и фундаментала
/// </summary>
internal sealed class GenerateSignalsStep(IDataStorageContext dataStorage) : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string StepName => "Генерация сигналов";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var portfolio = await dataStorage.PortfolioAssets.GetAllAsync();
        var indicatorData = portfolio
            .Select(a => (a.Ticker, Indicators: context.Indicators.GetValueOrDefault(a.Ticker)))
            .Where(a => a.Indicators != null)
            .ToList();
        foreach (var (ticker, indicators) in indicatorData)
        {
            var score = context.FundamentalScores.GetValueOrDefault(ticker, 0);
            var signal = GenerateSignal(ticker, indicators!, score, now);
            if (signal is not null)
            {
                await dataStorage.TradeSignals.AddAsync(signal);
            }
        }
    }

    private static TradeSignal? GenerateSignal(Ticker ticker, List<TechnicalIndicator> indicators, int score, DateTime now)
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

        switch (rsi.Value)
        {
            // BUY
            case < 30 when macd.Value > 0 && ema.Value > sma.Value && adx.Value > 20:
                action = TradeAction.Buy;
                confidence = score >= 4
                    ? ConfidenceLevel.High
                    : ConfidenceLevel.Medium;

                break;
            case < 30:
                action = TradeAction.Buy;
                confidence = ConfidenceLevel.Low;
                break;

            // SELL
            case > 70 when macd.Value < 0 && adx.Value > 20:
                action = TradeAction.Sell;
                confidence = ConfidenceLevel.High;
                break;
            case > 70:
                action = TradeAction.Sell;
                confidence = ConfidenceLevel.Medium;
                break;
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
