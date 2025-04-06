using BlackCandle.Application.Interfaces.Infrastructure;
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
/// <inheritdoc cref="TinkoffPortfolioClient" />
internal sealed class TinkoffPortfolioClient(IOptions<TinkoffClientConfiguration> config, ILoggerService logger, ITinkoffInvestApiClientWrapper investApiClient) : IPortfolioClient
{
    private readonly OperationsService.OperationsServiceClient _client = investApiClient.Operations;
    private readonly InstrumentsService.InstrumentsServiceClient _instrumentsClient = investApiClient.Instruments;
    private readonly TinkoffClientConfiguration _config = config.Value;

    /// <inheritdoc />
    public async Task<List<PortfolioAsset>> GetPortfolioAsync()
    {
        try
        {
            var positions = await _client.GetPortfolioAsync(new PortfolioRequest()
            {
                AccountId = _config.AccountId,
            });

            var assets = new List<PortfolioAsset>();

            foreach (var position in positions.Positions)
            {
                if (position.Quantity.Units == 0 && position.Quantity.Nano == 0)
                {
                    continue;
                }

                var figi = position.Figi;
                var instrument = _instrumentsClient.ShareBy(new InstrumentRequest
                {
                    Id = figi,
                    IdType = InstrumentIdType.Figi,
                });

                var ticker = new Ticker
                {
                    Symbol = instrument.Instrument.Ticker,
                    Currency = instrument.Instrument.Currency,
                    Sector = instrument.Instrument.Sector,
                    Figi = figi,
                };

                var quantity = position.Quantity.Units + (position.Quantity.Nano / 1_000_000_000M);

                var asset = new PortfolioAsset
                {
                    Ticker = ticker,
                    Quantity = quantity,
                    CurrentValue = position.CurrentPrice.ToDecimal(),
                };

                assets.Add(asset);
            }

            return assets;
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при получении портфеля из Tinkoff API", ex);
            throw new TinkoffApiException("Ошибка при получении портфеля");
        }
    }
}
