using BlackCandle.Application.Interfaces;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;

namespace BlackCandle.Application.Services;

/// <inheritdoc cref="IPortfolioService" />
public class PortfolioService : IPortfolioService
{
    private readonly IDataStorageContext _dataStorage;

    /// <inheritdoc cref="PortfolioService" />
    public PortfolioService(IDataStorageContext dataStorage)
    {
        _dataStorage = dataStorage;
    }

    /// <inheritdoc />
    public async Task<List<PortfolioAsset>> GetCurrentPortfolioAsync()
    {
        var assets = await _dataStorage.PortfolioAssets.GetAllAsync();
        if (assets.Count == 0)
        {
            throw new EmptyPortfolioException();
        }

        return assets;
    }

    /// <inheritdoc />
    public Task AddAssetAsync(PortfolioAsset asset)
    {
        return _dataStorage.PortfolioAssets.AddAsync(asset);
    }

    /// <inheritdoc />
    public Task RemoveAssetAsync(string tickerSymbol)
    {
        return _dataStorage.PortfolioAssets.RemoveAsync(tickerSymbol);
    }
}