using Tinkoff.InvestApi;
using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <inheritdoc cref="ITinkoffInvestApiClientWrapper"/>
internal class TinkoffInvestApiClientWrapper : ITinkoffInvestApiClientWrapper
{
    private readonly InvestApiClient _real;

    /// <inheritdoc cref="TinkoffInvestApiClientWrapper"/>
    public TinkoffInvestApiClientWrapper(InvestApiClient real)
    {
        _real = real;
    }

    /// <inheritdoc />
    public OperationsStreamService.OperationsStreamServiceClient OperationsStream => _real.OperationsStream;

    /// <inheritdoc />
    public OrdersService.OrdersServiceClient Orders => _real.Orders;

    /// <inheritdoc />
    public OrdersStreamService.OrdersStreamServiceClient OrdersStream => _real.OrdersStream;

    /// <inheritdoc />
    public SandboxService.SandboxServiceClient Sandbox => _real.Sandbox;

    /// <inheritdoc />
    public StopOrdersService.StopOrdersServiceClient StopOrders => _real.StopOrders;

    /// <inheritdoc />
    public UsersService.UsersServiceClient Users => _real.Users;

    /// <inheritdoc />
    public InstrumentsService.InstrumentsServiceClient Instruments => _real.Instruments;

    /// <inheritdoc />
    public MarketDataService.MarketDataServiceClient MarketData => _real.MarketData;

    /// <inheritdoc />
    public MarketDataStreamService.MarketDataStreamServiceClient MarketDataStream => _real.MarketDataStream;

    /// <inheritdoc />
    public OperationsService.OperationsServiceClient Operations => _real.Operations;
}
