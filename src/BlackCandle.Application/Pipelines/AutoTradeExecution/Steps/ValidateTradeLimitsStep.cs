using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для валидации сигналов
/// </summary>
internal sealed class ValidateTradeLimitsStep(
    IDataStorageContext dataStorage,
    ITradeLimitValidator validator) : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string Name => "Валидация сигналов";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        AutoTradeExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var portfolio = await dataStorage.PortfolioAssets.GetAllAsync();

        var validSignals = new List<TradeSignal>();
        foreach (var signal in context.Signals)
        {
            if (await validator.Validate(signal, portfolio))
            {
                validSignals.Add(signal);
            }
        }

        context.Signals = validSignals;
    }
}
