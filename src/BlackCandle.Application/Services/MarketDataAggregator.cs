using BlackCandle.Application.Interfaces;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Services;

/// <summary>
///     Агрегатор рыночных данных.
///     Выбирает источник, тянет OHLCV.
/// </summary>
public class MarketDataAggregator
{
    private readonly ITinkoffClient _tinkoff;

    /// <inheritdoc cref="MarketDataAggregator" />
    public MarketDataAggregator(ITinkoffClient tinkoff)
    {
        _tinkoff = tinkoff;
    }

    /// <summary>
    ///     Получить исторические данные в интервале
    /// </summary>
    public async Task<List<PriceHistoryPoint>> FetchHistoricalDataAsync(Ticker ticker, DateTime from, DateTime to)
    {
        return await _tinkoff.GetHistoricalDataAsync(ticker, from, to);
    }
}