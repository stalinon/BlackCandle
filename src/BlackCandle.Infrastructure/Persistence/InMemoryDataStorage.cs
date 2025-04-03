using BlackCandle.Application.Interfaces;
using BlackCandle.Domain.Entities;

namespace BlackCandle.Infrastructure.Persistence;

/// <summary>
///     Хранилище в оперативной памяти
/// </summary>
public class InMemoryDataStorage : IDataStorage
{
    private static readonly List<PortfolioAsset> Portfolio = new();

    /// <inheritdoc />
    public Task<List<PortfolioAsset>> GetPortfolioAsync()
    {
        // Возвращаем копию, чтобы не было утечек мутаций
        return Task.FromResult(Portfolio.Select(Clone).ToList());
    }

    /// <inheritdoc />
    public Task AddAssetAsync(PortfolioAsset asset)
    {
        Portfolio.Add(Clone(asset));
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAssetAsync(string tickerSymbol)
    {
        Portfolio.RemoveAll(x => x.Ticker.Symbol.Equals(tickerSymbol, StringComparison.OrdinalIgnoreCase));
        return Task.CompletedTask;
    }

    private PortfolioAsset Clone(PortfolioAsset asset)
    {
        return new PortfolioAsset
        {
            Ticker = new Ticker
            {
                Symbol = asset.Ticker.Symbol,
                Exchange = asset.Ticker.Exchange
            },
            Quantity = asset.Quantity,
            PurchasePrice = asset.PurchasePrice,
            PurchaseDate = asset.PurchaseDate
        };
    }
}