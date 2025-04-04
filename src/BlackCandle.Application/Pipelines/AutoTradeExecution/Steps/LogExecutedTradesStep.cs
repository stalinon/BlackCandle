using System.Text;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для логирования совершенных сделок
/// </summary>
internal sealed class LogExecutedTradesStep : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string StepName => "Логирование";

    /// <inheritdoc />
    public override async Task ExecuteAsync(AutoTradeExecutionContext context, CancellationToken cancellationToken = default)
    {
        var success = context.ExecutedTrades
            .Where(x => x.Status == TradeStatus.Success)
            .ToList();

        var failed = context.ExecutedTrades
            .Where(x => x.Status == TradeStatus.Error)
            .ToList();

        if (success.Count == 0 && failed.Count == 0)
        {
            return;
        }

        throw new NotImplementedException();
    }

    private static string BuildTelegramReport(List<ExecutedTrade> success, List<ExecutedTrade> failed)
    {
        var sb = new StringBuilder();
        sb.AppendLine("📦 *Auto-Trade Execution Report*");
        sb.AppendLine($"🕒 Время: {DateTime.UtcNow:dd.MM.yyyy HH:mm}");
        sb.AppendLine();

        if (success.Count > 0)
        {
            sb.AppendLine("✅ *Успешные сделки:*");
            foreach (var trade in success)
            {
                var emoji = trade.Side == TradeAction.Buy ? "🟢" : "🔴";
                sb.AppendLine($"{emoji} `{trade.Ticker.Symbol}` {trade.Side} {trade.Quantity} @ {trade.Price:F2}");
            }
        }

        if (failed.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("❌ *Ошибки исполнения:*");
            foreach (var trade in failed)
            {
                sb.AppendLine($"⚠️ `{trade.Ticker.Symbol}` {trade.Side} {trade.Quantity}");
            }
        }

        return sb.ToString();
    }
}
