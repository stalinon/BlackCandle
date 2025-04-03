using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces;

/// <summary>
///     Контекст хранилища данных, агрегирующий все репозитории
/// </summary>
public interface IDataStorageContext
{
    /// <summary>
    ///     Репозиторий портфеля
    /// </summary>
    IRepository<PortfolioAsset> PortfolioAssets { get; }
}