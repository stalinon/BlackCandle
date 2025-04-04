using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

/// <summary>
///     Шаг для размещения заявок
/// </summary>
internal sealed class PlaceOrdersStep(IInvestApiFacade investApi) : PipelineStep<AutoTradeExecutionContext>
{
    /// <inheritdoc />
    public override string Name => "Размещение заявок";

    /// <inheritdoc />
    public override async Task ExecuteAsync(
        AutoTradeExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        foreach (var trade in context.ExecutedTrades)
        {
            try
            {
                // если пайплайн исполняется в режиме превью, то не выставляем заявки
                if (context.PreviewMode)
                {
                    context.PreviewOrders.Add(new OrderPreview(trade.Ticker, trade.Side, trade.Quantity, trade.Price));
                    continue;
                }

                Logger.LogInfo($"Отправка заявки: {trade.Side} {trade.Quantity} {trade.Ticker.Symbol}");

                var price = await investApi.Trading.PlaceMarketOrderAsync(
                    trade.Ticker,
                    trade.Quantity,
                    trade.Side);

                trade.Price = price;
                trade.Status = TradeStatus.Success;

                Logger.LogInfo($"Заявка исполнена: {trade.Ticker.Symbol} по {price:F2}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при исполнении заявки для {trade.Ticker.Symbol}", ex);

                trade.Status = TradeStatus.Error;
                trade.Price = 0;
            }
        }
    }
}
