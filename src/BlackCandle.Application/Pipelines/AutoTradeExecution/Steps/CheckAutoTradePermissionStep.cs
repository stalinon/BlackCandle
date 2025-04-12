using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Exceptions;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для проверки разрешения на исполнение автоматической торговли
/// </summary>
internal sealed class CheckAutoTradePermissionStep(IBotSettingsService botSettingsService)
    : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string Name => "Проверка разрешения";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        AutoTradeExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var botSettings = await botSettingsService.GetAsync();
        if (!botSettings.EnableAutoTrading)
        {
            var exception = new AutoTradeNotEnabledException();
            EarlyExitAction.Invoke(context, exception, this);
        }
    }
}
