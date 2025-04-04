using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для вычисления объемов сделки
/// </summary>
internal sealed class CalculateTradeVolumeStep(ITradeExecutionService executionService) : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string StepName => "Вычисление объемов сделок";
    
    /// <inheritdoc />
    public override Task ExecuteAsync(AutoTradeExecutionContext context, CancellationToken cancellationToken = default)
    {
        var trades = new List<ExecutedTrade>();

        foreach (var signal in context.Signals)
        {
            var qty = executionService.CalculateVolume(signal);
            if (qty <= 0)
            {
                continue;
            }

            var trade = new ExecutedTrade
            {
                Ticker = signal.Ticker,
                Side = signal.Action,
                Quantity = qty,
                Price = 0m,
                ExecutedAt = DateTime.UtcNow,
                Status = TradeStatus.Pending
            };

            trades.Add(trade);
        }

        context.ExecutedTrades = trades;

        return Task.CompletedTask;
    }
}