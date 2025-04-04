using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Exceptions;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Реализация <see cref="ITradingClient"/> через Tinkoff Invest API
/// </summary>
internal sealed class TinkoffTradingClient : ITradingClient
{
    private readonly OrdersService.OrdersServiceClient _ordersClient;
    private readonly TinkoffClientConfiguration _config;
    private readonly ILoggerService _logger;

    /// <inheritdoc cref="TinkoffTradingClient"/>
    public TinkoffTradingClient(IOptions<TinkoffClientConfiguration> config, ILoggerService logger, InvestApiClient investApiClient)
    {
        _config = config.Value;
        _logger = logger;

        _ordersClient = investApiClient.Orders;
    }

    /// <inheritdoc />
    public async Task<decimal> PlaceMarketOrderAsync(Ticker ticker, decimal quantity, TradeAction side)
    {
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
                    _ => throw new ArgumentOutOfRangeException(nameof(side), "Неверное направление сделки")
                },
                AccountId = _config.AccountId,
                OrderType = OrderType.Market,
                InstrumentId = ticker.Figi,
                OrderId = Guid.NewGuid().ToString()
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
            _logger.LogError("Ошибка при выставлении заявки через Tinkoff API", ex);
            throw new TinkoffApiException("Не удалось выставить заявку на рынок");
        }
    }
}
