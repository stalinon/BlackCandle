using BlackCandle.Domain.Entities;

namespace BlackCandle.Domain.ValueObjects;

/// <summary>
///     Портфолио
/// </summary>
public class Portfolio
{
    private List<PortfolioAsset> Assets { get; } = new();

    public Portfolio()
    { }

    public Portfolio(List<PortfolioAsset> assets)
    {
        Assets = assets;
    }

    /// <summary>
    ///     Добавить
    /// </summary>
    public void AddAsset(PortfolioAsset asset)
    {
        Assets.Add(asset);
    }

    /// <summary>
    ///     Удалить по символу
    /// </summary>
    public void RemoveAsset(string symbol)
    {
        Assets.RemoveAll(x => x.Ticker.Symbol == symbol);
    }
}