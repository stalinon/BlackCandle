using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces.InvestApi;

/// <summary>
///     Клиент управления портфолио
/// </summary>
public interface IPortfolioClient
{
    /// <summary>
    ///     Получить портфолио
    /// </summary>
    Task<List<PortfolioAsset>> GetPortfolioAsync();
}