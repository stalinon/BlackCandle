using System.Text;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;

/// <summary>
///     Логирование
/// </summary>
internal sealed class LogStep(ITelegramService telegram, IDataStorageContext dataStorage)
    : PipelineStep<PortfolioAnalysisContext>
{
    /// <inheritdoc />
    public override string Name => "Логирование";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        PortfolioAnalysisContext context,
        CancellationToken cancellationToken = default)
    {
        var signals = await dataStorage.TradeSignals.GetAllAsync(s => s.Date.Date == DateTime.Now.Date);
        var date = context.AnalysisTime;

        var report = BuildTelegramReport(signals, date);
        await telegram.SendMessageAsync(report);
    }

    private static string BuildTelegramReport(List<TradeSignal> signals, DateTime date)
    {
        if (signals.Count == 0)
        {
            return $"📉 *Portfolio Analysis Report*\nДата: {date:dd.MM.yyyy HH:mm}\n\nНет торговых сигналов.";
        }

        var sb = new StringBuilder();
        sb.AppendLine("📊 *Portfolio Analysis Report*");
        sb.AppendLine($"🗓 Дата: {date:dd.MM.yyyy HH:mm}");
        sb.AppendLine();
        sb.AppendLine("*Сигналы:*");

        foreach (var s in signals.OrderByDescending(s => s.Confidence))
        {
            var emoji = s.Action switch
            {
                TradeAction.Buy => "🟢",
                TradeAction.Sell => "🔴",
                _ => "⚪",
            };

            var conf = s.Confidence switch
            {
                ConfidenceLevel.High => "🔥",
                ConfidenceLevel.Medium => "✅",
                _ => "⚠️",
            };

            sb.AppendLine($"{emoji} `{s.Ticker}` → *{s.Action}* {conf}");
            sb.AppendLine($"_Фундаментальный анализ: {s.Score}");
            sb.AppendLine($"_Причина: {s.Reason}_");
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }
}
