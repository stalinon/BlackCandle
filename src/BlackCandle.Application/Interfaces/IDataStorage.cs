using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces;

/// <summary>
///     Хранилище
/// </summary>
public interface IDataStorage
{
    /// <summary>
    ///     Получить портфолио
    /// </summary>
    Task<List<PortfolioAsset>> GetPortfolioAsync();
    
    /// <summary>
    ///     Добавить в портфолио
    /// </summary>
    Task AddAssetAsync(PortfolioAsset asset);
    
    /// <summary>
    ///     Удалить из портфолио
    /// </summary>
    Task RemoveAssetAsync(string tickerSymbol);
}