using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Обертка Tinkoff Invest
/// </summary>
public interface ITinkoffInvestApiClientWrapper
{
    /// <summary>
    ///     Инструменты
    /// </summary>
    InstrumentsService.InstrumentsServiceClient Instruments { get; }

    /// <summary>
    ///     Маркетдата
    /// </summary>
    MarketDataService.MarketDataServiceClient MarketData { get; }

    /// <summary>
    ///     Маркетдата-стрим
    /// </summary>
    MarketDataStreamService.MarketDataStreamServiceClient MarketDataStream { get; }

    /// <summary>
    ///     Операции
    /// </summary>
    OperationsService.OperationsServiceClient Operations { get; }

    /// <summary>
    ///     Операции-стрим
    /// </summary>
    OperationsStreamService.OperationsStreamServiceClient OperationsStream { get; }

    /// <summary>
    ///     Сделки
    /// </summary>
    OrdersService.OrdersServiceClient Orders { get; }

    /// <summary>
    ///     Сделки-стрим
    /// </summary>
    OrdersStreamService.OrdersStreamServiceClient OrdersStream { get; }

    /// <summary>
    ///     Песочница
    /// </summary>
    SandboxService.SandboxServiceClient Sandbox { get; }

    /// <summary>
    ///     Стоп-сделки
    /// </summary>
    StopOrdersService.StopOrdersServiceClient StopOrders { get; }

    /// <summary>
    ///     Пользователи
    /// </summary>
    UsersService.UsersServiceClient Users { get; }
}
