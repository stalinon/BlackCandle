using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Trading;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для валидации сигналов
/// </summary>
internal sealed class ValidateTradeLimitsStep(
    IDataStorageContext dataStorage,
    ITradeLimitValidator validator) : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string StepName => "Валидация сигналов";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        AutoTradeExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var portfolio = await dataStorage.PortfolioAssets.GetAllAsync();

        var validSignals = context.Signals.Where(signal => validator.Validate(signal, portfolio)).ToList();

        context.Signals = validSignals;
    }
}
