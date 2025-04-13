using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Infrastructure.Trading;

/// <inheritdoc cref="ITradeExecutionService" />
internal sealed class TradeExecutionService(
    IInvestApiFacade investApi,
    IBotSettingsService botSettingsService) : ITradeExecutionService
{
    /// <summary>
    ///     Посчитать объемы
    /// </summary>
    public async Task<decimal> CalculateVolume(TradeSignal signal)
    {
        if (signal.Action == TradeAction.Hold)
        {
            return 0;
        }

        var botSettings = await botSettingsService.GetAsync();
        var price = await investApi.Marketdata.GetCurrentPriceAsync(signal.Ticker);
        if (price is null or <= 0)
        {
            return 0;
        }

        var budget = signal.AllocatedCash ?? botSettings.TradeExecution.MaxTradeAmountRub;
        var rawQty = Math.Floor(budget / price.Value);

        if (rawQty <= 0)
        {
            return 0;
        }

        return Math.Min(rawQty, botSettings.TradeExecution.MaxLotsPerTrade);
    }
}
