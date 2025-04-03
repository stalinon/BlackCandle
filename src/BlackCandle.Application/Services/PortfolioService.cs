using BlackCandle.Application.Interfaces;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;

namespace BlackCandle.Application.Services;

/// <inheritdoc cref="IPortfolioService" />
public class PortfolioService : IPortfolioService
{
    private readonly IDataStorage _dataStorage;

    /// <inheritdoc cref="PortfolioService" />
    public PortfolioService(IDataStorage dataStorage)
    {
        _dataStorage = dataStorage;
    }

    /// <inheritdoc />
    public async Task<List<PortfolioAsset>> GetCurrentPortfolioAsync()
    {
        var assets = await _dataStorage.GetPortfolioAsync();
        if (assets.Count == 0)
        {
            throw new EmptyPortfolioException();
        }

        return assets;
    }

    /// <inheritdoc />
    public Task AddAssetAsync(PortfolioAsset asset)
    {
        return _dataStorage.AddAssetAsync(asset);
    }

    /// <inheritdoc />
    public Task RemoveAssetAsync(string tickerSymbol)
    {
        return _dataStorage.RemoveAssetAsync(tickerSymbol);
    }
}