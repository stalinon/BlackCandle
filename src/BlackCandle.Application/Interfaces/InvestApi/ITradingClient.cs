using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Interfaces.InvestApi;

/// <summary>
///     Клиент торговли
/// </summary>
public interface ITradingClient
{
    /// <summary>
    ///     Поставить заявку
    /// </summary>
    Task<string> PlaceMarketOrderAsync(Ticker ticker, decimal quantity, TradeAction side);
}