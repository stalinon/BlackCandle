using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces.Trading;

/// <summary>
///     Калькулятор для сделок
/// </summary>
public interface ITradeExecutionService
{
    /// <summary>
    ///     Рассчитать объем для потенциальной сделки
    /// </summary>
    Task<decimal> CalculateVolume(TradeSignal signal);
}
