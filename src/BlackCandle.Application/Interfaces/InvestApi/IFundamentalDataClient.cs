using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces.InvestApi;

/// <summary>
///     Клиент для получения фундаментальных данных
/// </summary>
public interface IFundamentalDataClient
{
    /// <summary>
    ///     Получить фундаментальные данные
    /// </summary>
    Task<FundamentalData?> GetFundamentalsAsync(Ticker ticker);
}
