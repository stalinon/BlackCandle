using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces.InvestApi;

/// <summary>
///     Клиент получения торговых данных
/// </summary>
public interface IMarketDataClient
{
    /// <summary>
    ///     Получить исторические данные в интервале
    /// </summary>
    Task<List<PriceHistoryPoint>> GetHistoricalDataAsync(Ticker ticker, DateTime from, DateTime to);
    
    /// <summary>
    ///     Получить текущую цену по тикеру
    /// </summary>
    Task<decimal?> GetCurrentPriceAsync(Ticker ticker);
}