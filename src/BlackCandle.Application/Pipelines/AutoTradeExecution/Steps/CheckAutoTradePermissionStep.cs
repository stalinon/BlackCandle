using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Exceptions;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для проверки разрешения на исполнение автоматической торговли
/// </summary>
internal sealed class CheckAutoTradePermissionStep(IDataStorageContext dataStorage) : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string StepName => "Проверка разрешения";

    /// <inheritdoc />
    public override async Task ExecuteAsync(AutoTradeExecutionContext context, CancellationToken cancellationToken = default)
    {
        var botSettings = await dataStorage.BotSettings.GetAllAsync();
        var settings = botSettings.Single();
        if (!settings.EnableAutoTrading)
        {
            var exception = new AutoTradeNotEnabledException();
            EarlyExitAction.Invoke(context, exception);
        }
    }
}