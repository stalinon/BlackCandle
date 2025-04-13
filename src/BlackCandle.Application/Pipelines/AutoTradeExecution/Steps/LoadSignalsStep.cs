using BlackCandle.Application.Filters;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для загрузки сигналов
/// </summary>
internal sealed class LoadSignalsStep(IDataStorageContext dataStorage) : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string Name => "Загрузка сигналов";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        AutoTradeExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var filter = new TradeSignalFilter
        {
            Date = today,
            OnlyBuySell = true,
        };
        var allSignals = await dataStorage.TradeSignals.GetAllAsync(filter);
        context.Signals = allSignals
            .Where(x => x.Action != TradeAction.Hold)
            .Where(x => x.Confidence != ConfidenceLevel.Low)
            .ToList();
    }
}
