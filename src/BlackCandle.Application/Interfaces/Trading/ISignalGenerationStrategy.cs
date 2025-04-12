using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Interfaces.Trading;

/// <summary>
///     Стратегия генерации сигналов на основе теханализа и фундаментала
/// </summary>
public interface ISignalGenerationStrategy
{
    /// <summary>
    ///     Генерировать сигнал
    /// </summary>
    TradeSignal? Generate(Ticker ticker, List<TechnicalIndicator> indicators, int fundamentalScore, DateTime now);
}
