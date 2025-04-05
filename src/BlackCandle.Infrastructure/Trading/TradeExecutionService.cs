using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

using Microsoft.Extensions.Options;

namespace BlackCandle.Infrastructure.Trading;

/// <inheritdoc cref="ITradeExecutionService" />
internal sealed class TradeExecutionService(
    IInvestApiFacade investApi,
    IOptions<TradeExecutionOptions> options) : ITradeExecutionService
{
    private readonly TradeExecutionOptions _config = options.Value;

    /// <inheritdoc />
    public decimal CalculateVolume(TradeSignal signal)
    {
        if (signal.Action == TradeAction.Hold)
        {
            return 0;
        }

        var price = investApi.Marketdata.GetCurrentPriceAsync(signal.Ticker).Result;
        if (price is null or <= 0)
        {
            return 0;
        }

        var maxBudget = _config.MaxTradeAmountRub;
        var rawQty = Math.Floor(maxBudget / price.Value);

        if (rawQty <= 0)
        {
            return 0;
        }

        var quantity = Math.Min(rawQty, _config.MaxLotsPerTrade);

        return quantity;
    }
}
