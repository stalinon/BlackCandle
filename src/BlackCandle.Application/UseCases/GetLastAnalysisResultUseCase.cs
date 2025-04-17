using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Возвращает текущие сигналы анализа портфеля
/// </summary>
public class GetLastAnalysisResultUseCase : IUseCase<IReadOnlyCollection<TradeSignal>>
{
    private readonly IDataStorageContext _storage = null!;

    /// <inheritdoc cref="GetLastAnalysisResultUseCase" />
    public GetLastAnalysisResultUseCase(IDataStorageContext storage)
    {
        _storage = storage;
    }

    /// <inheritdoc cref="GetLastAnalysisResultUseCase" />
    protected GetLastAnalysisResultUseCase()
    {
    }

    /// <inheritdoc />
    public virtual async Task<OperationResult<IReadOnlyCollection<TradeSignal>>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var signals = await _storage.TradeSignals.GetAllAsync();

            return signals.Any()
                ? OperationResult<IReadOnlyCollection<TradeSignal>>.Success(signals)
                : OperationResult<IReadOnlyCollection<TradeSignal>>.Failure("Нет сигналов анализа");
        }
        catch (Exception ex)
        {
            return OperationResult<IReadOnlyCollection<TradeSignal>>.Failure(ex.Message);
        }
    }
}
