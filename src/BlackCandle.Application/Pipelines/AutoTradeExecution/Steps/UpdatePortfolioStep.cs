using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг обновления портфеля по результатам исполненных сделок
/// </summary>
internal sealed class UpdatePortfolioStep(IDataStorageContext dataStorage) : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string Name => "Обновление портфеля";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        AutoTradeExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var successfulTrades = context.ExecutedTrades
            .Where(t => t.Status == TradeStatus.Success)
            .ToList();

        if (successfulTrades.Count == 0)
        {
            return;
        }

        var currentAssets = await dataStorage.PortfolioAssets.GetAllAsync();

        foreach (var trade in successfulTrades)
        {
            var asset = currentAssets.FirstOrDefault(a => a.Ticker.Symbol == trade.Ticker.Symbol);
            var updated = ProcessTrade(trade, asset);

            if (updated is null)
            {
                continue;
            }

            await SaveOrRemoveAsync(updated);
        }

        await dataStorage.ExecutedTrades.AddRangeAsync(successfulTrades);
    }

    private static PortfolioAsset? ProcessTrade(ExecutedTrade trade, PortfolioAsset? asset)
    {
        return asset == null
            ? CreateAssetFromTrade(trade)
            : UpdateAssetFromTrade(trade, asset);
    }

    private static PortfolioAsset? CreateAssetFromTrade(ExecutedTrade trade)
    {
        return trade.Side == TradeAction.Buy
            ? new PortfolioAsset
            {
                Ticker = trade.Ticker,
                Quantity = trade.Quantity,
                CurrentValue = trade.Price,
            }
            : null;
    }

    private static PortfolioAsset UpdateAssetFromTrade(ExecutedTrade trade, PortfolioAsset asset)
    {
        switch (trade.Side)
        {
            case TradeAction.Buy:
                var total = asset.Quantity + trade.Quantity;
                asset.CurrentValue = ((asset.CurrentValue * asset.Quantity) + (trade.Price * trade.Quantity)) / total;
                asset.Quantity = total;
                break;

            case TradeAction.Sell:
                asset.Quantity -= trade.Quantity;
                if (asset.Quantity < 0)
                {
                    asset.Quantity = 0;
                }

                break;
        }

        return asset;
    }

    private async Task SaveOrRemoveAsync(PortfolioAsset asset)
    {
        if (asset.Quantity == 0)
        {
            await dataStorage.PortfolioAssets.RemoveAsync(asset.Id);
            return;
        }

        await dataStorage.PortfolioAssets.AddAsync(asset);
    }
}
