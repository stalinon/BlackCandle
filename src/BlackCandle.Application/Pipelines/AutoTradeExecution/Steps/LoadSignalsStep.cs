using BlackCandle.Application.Interfaces;
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
        var allSignals = await dataStorage.TradeSignals.GetAllAsync(s =>
            s.Date.Date == today && (s.Action == TradeAction.Buy || s.Action == TradeAction.Sell));

        context.Signals = allSignals;
    }
}
