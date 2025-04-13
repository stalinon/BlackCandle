using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для вычисления объемов сделки
/// </summary>
internal sealed class CalculateTradeVolumeStep(ITradeExecutionService executionService, IInvestApiFacade api, IDataStorageContext dataStorage)
    : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string Name => "Вычисление объемов сделок";

    /// <inheritdoc />
    public override async Task ExecuteAsync(AutoTradeExecutionContext context, CancellationToken cancellationToken = default)
    {
        var signals = context.Signals;
        if (signals.Count == 0)
        {
            return;
        }

        var trades = new List<ExecutedTrade>();
        var portfolio = await dataStorage.PortfolioAssets.GetAllAsync();

        // Получаем текущий кэш
        var currentCash = await api.Portfolio.GetAvailableCashAsync();

        // Ожидаемый доход от продаж
        foreach (var signal in signals.Where(s => s.Action == TradeAction.Sell))
        {
            var asset = portfolio.FirstOrDefault(p => p.Ticker.Symbol == signal.Ticker.Symbol);
            if (asset is null || asset.Quantity <= 0)
            {
                continue;
            }

            var price = await api.Marketdata.GetCurrentPriceAsync(signal.Ticker);
            if (!price.HasValue)
            {
                continue;
            }

            currentCash += price.Value * asset.Quantity;
        }

        // Берем только сигналы на покупку
        var buySignals = signals.Where(s => s.Action == TradeAction.Buy).ToList();
        var scores = buySignals.Select(s => s.FundamentalScore + s.TechnicalScores.Sum(t => t.Score)).ToList();
        var totalScore = scores.Sum();

        if (totalScore == 0)
        {
            context.ExecutedTrades = [];
            return;
        }

        // Распределяем кэш
        for (var i = 0; i < buySignals.Count; i++)
        {
            var signal = buySignals[i];
            var portion = scores[i] / (decimal)totalScore;
            signal.AllocatedCash = currentCash * portion;
        }

        // Считаем объемы
        foreach (var signal in signals)
        {
            var qty = await executionService.CalculateVolume(signal);
            if (qty <= 0)
            {
                continue;
            }

            trades.Add(new ExecutedTrade
            {
                Ticker = signal.Ticker,
                Side = signal.Action,
                Quantity = qty,
                Price = 0m,
                ExecutedAt = DateTime.UtcNow,
                Status = TradeStatus.Pending,
            });
        }

        context.ExecutedTrades = trades;
    }
}
