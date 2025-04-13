using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.ValueObjects;

using Skender.Stock.Indicators;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Расчет технических индикаторов (SMA, EMA, RSI, MACD, ADX)
/// </summary>
internal sealed class CalculateIndicatorsStep(IDataStorageContext dataStorage) : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string Name => "Расчет тех. индикаторов";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var marketdata = await dataStorage.Marketdata.GetAllAsync();
        var groupedMarketdata = marketdata.GroupBy(m => m.Ticker)
            .ToDictionary(kv => kv.Key, kv => kv.ToList());
        foreach (var (ticker, candles) in groupedMarketdata)
        {
            if (candles.Count < 50)
            {
                continue;
            }

            var quotes = candles.OrderBy(c => c.Date).Select(c => new Quote
            {
                Timestamp = c.Date,
                Open = c.Open,
                High = c.High,
                Low = c.Low,
                Close = c.Close,
                Volume = c.Volume,
            }).ToList();

            var indicators = new List<TechnicalIndicator>();

            // 1. SMA(20)
            var sma = quotes.ToSma(20);
            indicators.AddRange(sma.Select(s => new TechnicalIndicator
            {
                Name = "SMA20",
                Date = s.Timestamp,
                Value = s.Sma,
            }));

            // 2. EMA(12)
            var ema = quotes.ToEma(12);
            indicators.AddRange(ema.Select(e => new TechnicalIndicator
            {
                Name = "EMA12",
                Date = e.Timestamp,
                Value = e.Ema,
            }));

            // 3. RSI(14)
            var rsi = quotes.ToRsi();
            indicators.AddRange(rsi.Select(r => new TechnicalIndicator
            {
                Name = "RSI14",
                Date = r.Timestamp,
                Value = r.Rsi,
            }));

            // 4. MACD(12/26/9)
            var macd = quotes.ToMacd();
            indicators.AddRange(macd.Select(m => new TechnicalIndicator
            {
                Name = "MACD",
                Date = m.Timestamp,
                Value = m.Macd,
                Meta = $"Signal={m.Signal}, Histogram={m.Histogram}",
            }));

            // 5. ADX(14)
            var adx = quotes.ToAdx();
            indicators.AddRange(adx.Select(a => new TechnicalIndicator
            {
                Name = "ADX14",
                Date = a.Timestamp,
                Value = a.Adx,
            }));

            // 6. CLOSE
            indicators.AddRange(quotes.Select(a => new TechnicalIndicator
            {
                Name = "CLOSE",
                Date = a.Timestamp,
                Value = (double?)a.Close,
            }));

            context.Indicators[ticker] = indicators
                .Where(i => i.Value.HasValue)
                .GroupBy(i => i.Date)
                .SelectMany(g => g)
                .ToList();
        }
    }
}
