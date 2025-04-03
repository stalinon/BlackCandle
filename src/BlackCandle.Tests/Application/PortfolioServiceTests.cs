using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Services;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using Moq;

namespace BlackCandle.Tests.Application;

/// <summary>
///     Тесты на <see cref="PortfolioService"/>
/// </summary>
public class PortfolioServiceTests
{
    private readonly Mock<IDataStorage> _mockStorage;
    private readonly PortfolioService _service;

    public PortfolioServiceTests()
    {
        _mockStorage = new Mock<IDataStorage>();
        _service = new PortfolioService(_mockStorage.Object);
    }

    /// <summary>
    ///     Тест 1: Получение текущего портфолио. Если портфель пуст, то выбрасывает исключение
    /// </summary>
    [Fact]
    public async Task GetCurrentPortfolioAsync_ShouldThrow_WhenPortfolioEmpty()
    {
        _mockStorage.Setup(x => x.GetPortfolioAsync()).ReturnsAsync(new List<PortfolioAsset>());

        await Assert.ThrowsAsync<EmptyPortfolioException>(() => _service.GetCurrentPortfolioAsync());
    }

    /// <summary>
    ///     Тест 2: Получение текущего портфеля. Если портфель не пуст, то возвращает активы
    /// </summary>
    [Fact]
    public async Task GetCurrentPortfolioAsync_ShouldReturnAssets_WhenPortfolioNotEmpty()
    {
        var assets = new List<PortfolioAsset> {
            new PortfolioAsset {
                Ticker = new Ticker { Symbol = "SBER", Exchange = "MOEX" },
                Quantity = 10, PurchasePrice = 200, PurchaseDate = DateTime.Today
            }
        };

        _mockStorage.Setup(x => x.GetPortfolioAsync()).ReturnsAsync(assets);

        var result = await _service.GetCurrentPortfolioAsync();

        Assert.Single(result);
        Assert.Equal("SBER", result[0].Ticker.Symbol);
    }

    /// <summary>
    ///     Тест 3: Добавление нового актива. Должен дергать хранилище
    /// </summary>
    [Fact]
    public async Task AddAssetAsync_ShouldCallStorage()
    {
        var asset = new PortfolioAsset
        {
            Ticker = new Ticker { Symbol = "GAZP", Exchange = "MOEX" },
            Quantity = 5, PurchasePrice = 300, PurchaseDate = DateTime.Today
        };

        await _service.AddAssetAsync(asset);

        _mockStorage.Verify(x => x.AddAssetAsync(asset), Times.Once);
    }
}