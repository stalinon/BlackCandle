using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Infrastructure.InvestApi.Tinkoff.Extensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Реализация <see cref="IMarketDataClient" /> для Tinkoff API
/// </summary>
internal sealed class TinkoffMarketDataClient : IMarketDataClient
{
    private readonly ILoggerService _logger;
    private readonly MarketDataService.MarketDataServiceClient _client;
    
    /// <inheritdoc cref="TinkoffMarketDataClient" />
    public TinkoffMarketDataClient(IOptions<TinkoffClientConfiguration> config, ILoggerService logger)
    {
        _logger = logger;
        _client = InvestApiClientFactory.Create(config.Value.ApiKey).MarketData;
    }
    
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
                InstrumentId = ticker.Figi
            });

            return response.Candles.Select(c => new PriceHistoryPoint
            {
                Date = c.Time.ToDateTime(),
                Open = c.Open.ToDecimal(),
                High = c.High.ToDecimal(),
                Low = c.Low.ToDecimal(),
                Close = c.Close.ToDecimal(),
                Volume = c.Volume
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка при получении исторических данных от Tinkoff", ex);
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
            _logger.LogError("Ошибка при получении текущей цены инструмента от Tinkoff", ex);
            throw new TinkoffApiException("Ошибка при получении текущей цены инструмента от Tinkoff API");
        }
    }
}