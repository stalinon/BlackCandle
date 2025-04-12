using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces.InvestApi;

/// <summary>
///     Клиент получения данных об инструментах
/// </summary>
public interface IInstrumentClient
{
    /// <summary>
    ///     Получить список топовых тикеров
    /// </summary>
    Task<IEnumerable<Ticker>> GetTopTickersAsync(int count);
}
