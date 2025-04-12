using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Domain.Helpers;

using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Реализация <see cref="ITradingClient"/> через Tinkoff Invest API
/// </summary>
/// <inheritdoc cref="TinkoffTradingClient"/>
internal sealed class TinkoffTradingClient(IBotSettingsService botSettingsService, ILoggerService logger, ITinkoffInvestApiClientWrapper investApiClient) : ITradingClient
{
    private readonly OrdersService.OrdersServiceClient _ordersClient = investApiClient.Orders;

    /// <inheritdoc />
    public async Task<decimal> PlaceMarketOrderAsync(Ticker ticker, decimal quantity, TradeAction side)
    {
        var config = await botSettingsService.GetAsync();
        var tinkoffConfig = config.ToTinkoffConfig();

        try
        {
            var orderRequest = new PostOrderRequest
            {
                Quantity = (int)quantity,
                Price = null, // TODO
                Direction = side switch
                {
                    TradeAction.Buy => OrderDirection.Buy,
                    TradeAction.Sell => OrderDirection.Sell,
                    _ => throw new ArgumentOutOfRangeException(nameof(side), "Неверное направление сделки"),
                },
                AccountId = tinkoffConfig.AccountId,
                OrderType = OrderType.Market,
                InstrumentId = ticker.Figi,
                OrderId = Guid.NewGuid().ToString(),
            };

            var result = await _ordersClient.PostOrderAsync(orderRequest);
            return result.ExecutedOrderPrice;
        }
        catch (ArgumentOutOfRangeException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError("Ошибка при выставлении заявки через Tinkoff API", ex);
            throw new TinkoffApiException("Не удалось выставить заявку на рынок");
        }
    }
}
