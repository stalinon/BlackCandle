using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

using Microsoft.Extensions.Options;

namespace BlackCandle.Infrastructure.Trading;

/// <inheritdoc cref="ITradeLimitValidator" />
internal sealed class TradeLimitValidator(
    IInvestApiFacade investApi,
    IOptions<TradeLimitOptions> options) : ITradeLimitValidator
{
    private readonly TradeLimitOptions _config = options.Value;

    /// <inheritdoc />
    public bool Validate(TradeSignal signal, List<PortfolioAsset> portfolio)
    {
        if (signal.Action != TradeAction.Buy)
        {
            return true;
        }

        // Получаем текущую цену
        var price = investApi.Marketdata.GetCurrentPriceAsync(signal.Ticker).Result;
        if (price is null or <= 0)
        {
            return false;
        }

        // Проверка на мин. сумму сделки
        if (price < _config.MinTradeAmountRub)
        {
            return false;
        }

        // Проверка на долю позиции в портфеле
        var totalValue = portfolio.Sum(p => p.Quantity * p.CurrentValue);
        var currentAsset = portfolio.FirstOrDefault(p => p.Ticker.Equals(signal.Ticker));
        var currentValue = currentAsset?.Quantity * currentAsset?.CurrentValue ?? 0m;

        var futureValue = currentValue + price.Value;
        var sharePercent = futureValue / (totalValue + price.Value) * 100m;

        return sharePercent < _config.MaxPositionSharePercent;
    }
}
