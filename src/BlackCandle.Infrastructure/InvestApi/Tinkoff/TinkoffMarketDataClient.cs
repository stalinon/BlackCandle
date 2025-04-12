using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Infrastructure.InvestApi.Tinkoff.Extensions;

using Google.Protobuf.WellKnownTypes;

using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Реализация <see cref="IMarketDataClient" /> для Tinkoff API
/// </summary>
internal sealed class TinkoffMarketDataClient(ILoggerService logger, ITinkoffInvestApiClientWrapper investApiClient) : IMarketDataClient
{
    private readonly MarketDataService.MarketDataServiceClient _client = investApiClient.MarketData;

    /// <inheritdoc />
    public async Task<List<PriceHistoryPoint>> GetHistoricalDataAsync(Ticker ticker, DateTime from, DateTime to)
    {
        try
        {
            var response = await _client.GetCandlesAsync(new GetCandlesRequest
            {
                Interval = CandleInterval.Day,
                From = Timestamp.FromDateTime(from.ToUniversalTime()),
                To = Timestamp.FromDateTime(to.ToUniversalTime()),
                InstrumentId = ticker.Figi,
            });

            return response.Candles.Select(c => new PriceHistoryPoint
            {
                Ticker = ticker,
                Date = c.Time.ToDateTime(),
                Open = c.Open.ToDecimal(),
                High = c.High.ToDecimal(),
                Low = c.Low.ToDecimal(),
                Close = c.Close.ToDecimal(),
                Volume = c.Volume,
            }).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при получении исторических данных от Tinkoff", ex);
            throw new TinkoffApiException("Ошибка при получении исторических данных от Tinkoff API");
        }
    }

    /// <inheritdoc />
    public async Task<decimal?> GetCurrentPriceAsync(Ticker ticker)
    {
        try
        {
            var prices = await _client.GetLastPricesAsync(new GetLastPricesRequest { InstrumentId = { ticker.Figi } });
            return prices.LastPrices.FirstOrDefault()?.Price.ToDecimal();
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при получении текущей цены инструмента от Tinkoff", ex);
            throw new TinkoffApiException("Ошибка при получении текущей цены инструмента от Tinkoff API");
        }
    }
}
