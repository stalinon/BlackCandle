using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
/// Возвращает текущие сигналы анализа портфеля
/// </summary>
internal sealed class GetLastAnalysisResultUseCase(IDataStorageContext storage)
    : IUseCase<IReadOnlyCollection<TradeSignal>>
{
    /// <inheritdoc />
    public async Task<OperationResult<IReadOnlyCollection<TradeSignal>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var signals = await storage.TradeSignals.GetAllAsync();

        return signals.Any()
            ? OperationResult<IReadOnlyCollection<TradeSignal>>.Success(signals)
            : OperationResult<IReadOnlyCollection<TradeSignal>>.Failure("Нет сигналов анализа");
    }
}
