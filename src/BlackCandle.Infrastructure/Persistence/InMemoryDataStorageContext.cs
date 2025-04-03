
using BlackCandle.Application.Interfaces;
using BlackCandle.Domain.Entities;

namespace BlackCandle.Infrastructure.Persistence;

/// <summary>
/// Контекст хранилища на основе памяти (in-memory)
/// </summary>
public class InMemoryDataStorageContext : IDataStorageContext
{
    /// <inheritdoc />
    public IRepository<PortfolioAsset> PortfolioAssets { get; } = new InMemoryRepository<PortfolioAsset>();
}