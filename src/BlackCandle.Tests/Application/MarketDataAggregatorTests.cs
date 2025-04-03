using BlackCandle.Application.Services;
using BlackCandle.Domain.Entities;
using BlackCandle.Tests.Mocking;

namespace BlackCandle.Tests.Application;

/// <summary>
///     Тесты для <see cref="MarketDataAggregatorTests" />
/// </summary>
public class MarketDataAggregatorTests
{
    /// <summary>
    ///     Тест 1: Получение исторических данных. Возвращает корректно.
    /// </summary>
    [Fact]
    public async Task FetchHistoricalData_ShouldReturnCorrectData()
    {
        // Arrange
        var fakeClient = new FakeTinkoffClient();
        var aggregator = new MarketDataAggregator(fakeClient);
        var ticker = new Ticker { Symbol = "SBER", Exchange = "MOEX" };
        var from = DateTime.Today.AddDays(-10);
        var to = DateTime.Today;

        // Act
        var result = await aggregator.FetchHistoricalDataAsync(ticker, from, to);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(100, result[0].Open);
        Assert.Equal(110, result[2].Close);
        Assert.All(result, point => Assert.True(point.Volume > 0));
    }

    /// <summary>
    ///     Тест 2: Получение исторических данных. Возвращает в интервале.
    /// </summary>
    [Fact]
    public async Task FetchHistoricalData_ShouldRespectDateRange()
    {
        var fakeClient = new FakeTinkoffClient();
        var aggregator = new MarketDataAggregator(fakeClient);
        var ticker = new Ticker { Symbol = "GAZP", Exchange = "MOEX" };
        var from = new DateTime(2023, 01, 01);
        var to = new DateTime(2023, 12, 31);

        var data = await aggregator.FetchHistoricalDataAsync(ticker, from, to);

        Assert.All(data, d =>
        {
            Assert.True(d.Date >= from);
            Assert.True(d.Date <= to.AddDays(3)); // мы сдвигаем +3 дня в фейковых данных
        });
    }
}