using BlackCandle.Domain.Entities;
using BlackCandle.Infrastructure.Persistence;

namespace BlackCandle.Tests.Infrastructure;

/// <summary>
///     Тесты <see cref="InMemoryDataStorage" />
/// </summary>
public class InMemoryDataStorageTests
{
    /// <summary>
    ///     Тест 1: Добавление актива. Реально добавляет
    /// </summary>
    [Fact]
    public async Task AddAssetAsync_StoresDataCorrectly()
    {
        var storage = new InMemoryDataStorage();
        var asset = new PortfolioAsset
        {
            Ticker = new Ticker { Symbol = "YNDX", Exchange = "MOEX" },
            Quantity = 100,
            PurchasePrice = 2500,
            PurchaseDate = DateTime.Today
        };

        await storage.AddAssetAsync(asset);
        var result = await storage.GetPortfolioAsync();

        Assert.Single(result);
        Assert.Equal("YNDX", result[0].Ticker.Symbol);
    }

    /// <summary>
    ///     Тест 2: Удаление актива. Реально удаляет
    /// </summary>
    [Fact]
    public async Task RemoveAssetAsync_RemovesCorrectly()
    {
        var storage = new InMemoryDataStorage();
        var asset = new PortfolioAsset
        {
            Ticker = new Ticker { Symbol = "AAPL", Exchange = "NASDAQ" },
            Quantity = 10,
            PurchasePrice = 150,
            PurchaseDate = DateTime.Today
        };

        await storage.AddAssetAsync(asset);
        await storage.RemoveAssetAsync("AAPL");

        var result = await storage.GetPortfolioAsync();
        Assert.Empty(result);
    }
}