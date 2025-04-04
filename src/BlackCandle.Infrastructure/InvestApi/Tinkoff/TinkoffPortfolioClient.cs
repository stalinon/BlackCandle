using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Infrastructure.InvestApi.Tinkoff.Extensions;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Реализация <see cref="IPortfolioClient" /> через Tinkoff Invest API
/// </summary>
internal sealed class TinkoffPortfolioClient : IPortfolioClient
{
    private readonly OperationsService.OperationsServiceClient _client;
    private readonly InstrumentsService.InstrumentsServiceClient _instrumentsClient;
    private readonly TinkoffClientConfiguration _config;
    private readonly ILoggerService _logger;

    /// <inheritdoc cref="TinkoffPortfolioClient" />
    public TinkoffPortfolioClient(IOptions<TinkoffClientConfiguration> config, ILoggerService logger)
    {
        _config = config.Value;
        _logger = logger;

        var client = InvestApiClientFactory.Create(_config.ApiKey);
        _client = client.Operations;
        _instrumentsClient = client.Instruments;
    }

    /// <inheritdoc />
    public async Task<List<PortfolioAsset>> GetPortfolioAsync()
    {
        try
        {
            var positions = await _client.GetPortfolioAsync(new PortfolioRequest()
            {
                AccountId = _config.AccountId
            });

            var assets = new List<PortfolioAsset>();

            foreach (var position in positions.Positions)
            {
                if (position.Quantity.Units == 0 && position.Quantity.Nano == 0)
                {
                    continue;
                }

                var figi = position.Figi;
                var instrument = _instrumentsClient.ShareBy(new()
                {
                    Id = figi,
                    IdType = InstrumentIdType.Figi
                });

                var ticker = new Ticker
                {
                    Symbol = instrument.Instrument.Ticker,
                    Currency = instrument.Instrument.Currency,
                    Sector = instrument.Instrument.Sector,
                    Figi = figi
                };

                var quantity = position.Quantity.Units + position.Quantity.Nano / 1_000_000_000M;
                var buyPrice = position.CurrentPrice.ToDecimal() * quantity;

                var asset = new PortfolioAsset
                {
                    Ticker = ticker,
                    Quantity = quantity,
                    CurrentValue = buyPrice
                };

                assets.Add(asset);
            }

            return assets;
        }
        catch (Exception ex)
        {
            _logger.LogError("Ошибка при получении портфеля из Tinkoff API", ex);
            throw new TinkoffApiException("Ошибка при получении портфеля");
        }
    }
}
