using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces;

/// <summary>
///     Сервис управления портфелем
/// </summary>
public interface IPortfolioService
{
    /// <summary>
    ///     Получить текущее состояние портфеля
    /// </summary>
    Task<List<PortfolioAsset>> GetCurrentPortfolioAsync();
    
    /// <summary>
    ///     Добавить в портфель
    /// </summary>
    Task AddAssetAsync(PortfolioAsset asset);
    
    /// <summary>
    ///     Удалить из портфеля
    /// </summary>
    Task RemoveAssetAsync(string tickerSymbol);
}