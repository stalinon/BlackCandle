using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Domain.Helpers;
using BlackCandle.Infrastructure.InvestApi.Tinkoff.Extensions;

using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Реализация <see cref="IPortfolioClient" /> через Tinkoff Invest API
/// </summary>
/// <inheritdoc cref="TinkoffPortfolioClient" />
internal sealed class TinkoffPortfolioClient(IBotSettingsService botSettingsService, ILoggerService logger, ITinkoffInvestApiClientWrapper investApiClient) : IPortfolioClient
{
    private readonly OperationsService.OperationsServiceClient _client = investApiClient.Operations;
    private readonly InstrumentsService.InstrumentsServiceClient _instrumentsClient = investApiClient.Instruments;

    /// <inheritdoc />
    public async Task<List<PortfolioAsset>> GetPortfolioAsync()
    {
        var botSettings = await botSettingsService.GetAsync();
        var config = botSettings.ToTinkoffConfig();

        try
        {
            var positions = await _client.GetPortfolioAsync(new PortfolioRequest
            {
                AccountId = config.AccountId,
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

                var quantity = position.Quantity.ToDecimal();

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

    /// <inheritdoc />
    public async Task<decimal> GetAvailableCashAsync()
    {
        var botSettings = await botSettingsService.GetAsync();
        var config = botSettings.ToTinkoffConfig();

        try
        {
            var positions = await _client.GetPortfolioAsync(new PortfolioRequest
            {
                AccountId = config.AccountId,
            });

            var cash = positions.Positions
                .Where(x => x.InstrumentType == "Currency" && x.Figi == "RU000A105EX7")
                .Sum(x => x.Quantity.ToDecimal());

            return cash;
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при получении текущего количество свободных денег из Tinkoff API", ex);
            throw new TinkoffApiException("Ошибка при получении текущего количество свободных денег");
        }
    }
}
