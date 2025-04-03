using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Interfaces;

/// <summary>
/// Источник данных с Tinkoff Invest API.
/// </summary>
public interface ITinkoffClient
{
    /// <summary>
    ///     Получить исторические данные по тикеру
    /// </summary>
    Task<List<PriceHistoryPoint>> GetHistoricalDataAsync(Ticker ticker, DateTime from, DateTime to);
}