using BlackCandle.Application.Interfaces;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Tests.Mocking;

/// <summary>
///     Мок-клиент для симуляции исторических данных от Tinkoff.
/// </summary>
public class FakeTinkoffClient : ITinkoffClient
{
    /// <inheritdoc />
    public Task<List<PriceHistoryPoint>> GetHistoricalDataAsync(Ticker ticker, DateTime from, DateTime to)
    {
        // Возвращаем фейковые OHLCV данные — типа за 3 дня
        var data = new List<PriceHistoryPoint>
        {
            new() { Date = from.AddDays(1), Open = 100, High = 110, Low = 95, Close = 105, Volume = 1000000 },
            new() { Date = from.AddDays(2), Open = 105, High = 112, Low = 101, Close = 108, Volume = 1500000 },
            new() { Date = from.AddDays(3), Open = 108, High = 115, Low = 102, Close = 110, Volume = 2000000 },
        };

        return Task.FromResult(data);
    }
}