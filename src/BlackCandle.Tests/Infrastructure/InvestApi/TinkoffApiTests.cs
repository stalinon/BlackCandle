using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Infrastructure.InvestApi.Tinkoff;

using FluentAssertions;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.Extensions.Options;

using Moq;

using Tinkoff.InvestApi.V1;

namespace BlackCandle.Tests.Infrastructure.InvestApi;

/// <summary>
///     Тесты для Tinkoff Invest API
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Должен вернуть цену, если GRPC вернул значение.</item>
///         <item>Должен выбросить исключение, если GRPC вернул ошибку при GetCurrentPriceAsync.</item>
///         <item>GetHistoricalDataAsync возвращает данные по свечам.</item>
///         <item>GetPortfolioAsync возвращает список активов.</item>
///         <item>PlaceMarketOrderAsync отправляет заявку и возвращает цену исполнения.</item>
///         <item>PlaceMarketOrderAsync должен выбросить ArgumentOutOfRangeException при неверном направлении.</item>
///         <item>GetHistoricalDataAsync должен выбросить исключение при ошибке GRPC.</item>
///         <item>GetPortfolioAsync должен выбросить исключение при ошибке GRPC.</item>
///         <item>PlaceMarketOrderAsync должен выбросить исключение при ошибке GRPC.</item>
///     </list>
/// </remarks>
public class TinkoffApiTests
{
    /// <summary>
    ///     Тест 1: Должен вернуть цену, если GRPC вернул значение
    /// </summary>
    [Fact(DisplayName = "Тест 1: Должен вернуть цену, если GRPC вернул значение")]
    public async Task GetCurrentPriceAsync_ShouldReturnPrice_WhenGrpcReturnsValue()
    {
        // Arrange
        var ticker = new Ticker { Figi = "figi123" };
        var grpcResponse = new GetLastPricesResponse
        {
            LastPrices = { new LastPrice { Price = new Quotation { Units = 123, Nano = 0 } } },
        };

        var marketDataMock = new Mock<MarketDataService.MarketDataServiceClient>();
        marketDataMock
            .Setup(x => x.GetLastPricesAsync(It.IsAny<GetLastPricesRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(() => new AsyncUnaryCall<GetLastPricesResponse>(
                Task.FromResult(grpcResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        var client = new TinkoffMarketDataClient(
            new Mock<ILoggerService>().Object,
            MockApiClient(marketData: marketDataMock.Object));

        // Act
        var result = await client.GetCurrentPriceAsync(ticker);

        // Assert
        result.Should().Be(123);
    }

    /// <summary>
    ///     Тест 2: Должен выбросить исключение, если GRPC вернул ошибку при GetCurrentPriceAsync
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен выбросить исключение, если GRPC вернул ошибку при GetCurrentPriceAsync")]
    public async Task GetCurrentPriceAsync_ShouldThrowException_WhenGrpcFails()
    {
        var marketDataMock = new Mock<MarketDataService.MarketDataServiceClient>();
        marketDataMock
            .Setup(x => x.GetLastPricesAsync(It.IsAny<GetLastPricesRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Throws(new RpcException(new Status(StatusCode.Internal, "error")));

        var loggerMock = new Mock<ILoggerService>();

        var client = new TinkoffMarketDataClient(
            loggerMock.Object,
            MockApiClient(marketData: marketDataMock.Object));

        var act = async () => await client.GetCurrentPriceAsync(new Ticker { Figi = "figi" });

        await act.Should().ThrowAsync<TinkoffApiException>();
    }

    /// <summary>
    ///     Тест 3: GetHistoricalDataAsync возвращает данные по свечам
    /// </summary>
    [Fact(DisplayName = "Тест 3: GetHistoricalDataAsync возвращает данные по свечам")]
    public async Task GetHistoricalDataAsync_ShouldReturnData()
    {
        var ticker = new Ticker { Figi = "figi" };
        var candles = new List<HistoricCandle>
        {
            new()
            {
                Time = Timestamp.FromDateTime(DateTime.UtcNow),
                Open = new Quotation { Units = 1 },
                High = new Quotation { Units = 2 },
                Low = new Quotation { Units = 0 },
                Close = new Quotation { Units = 1 },
                Volume = 100,
            },
        };

        var marketDataMock = new Mock<MarketDataService.MarketDataServiceClient>();
        marketDataMock
            .Setup(x => x.GetCandlesAsync(It.IsAny<GetCandlesRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(() => new AsyncUnaryCall<GetCandlesResponse>(
                Task.FromResult(new GetCandlesResponse { Candles = { candles } }),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        var client = new TinkoffMarketDataClient(
            new Mock<ILoggerService>().Object,
            MockApiClient(marketData: marketDataMock.Object));

        var result = await client.GetHistoricalDataAsync(ticker, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        result.Should().HaveCount(1);
        result.First().Open.Should().Be(1);
    }

    /// <summary>
    ///     Тест 4: GetPortfolioAsync возвращает список активов
    /// </summary>
    [Fact(DisplayName = "Тест 4: GetPortfolioAsync возвращает список активов")]
    public async Task GetPortfolioAsync_ShouldReturnAssets()
    {
        var portfolioResponse = new PortfolioResponse
        {
            Positions =
            {
                new PortfolioPosition
                {
                    Figi = "figi",
                    Quantity = new Quotation { Units = 1 },
                    CurrentPrice = new MoneyValue { Units = 10 },
                },
            },
        };

        var instrumentResponse = new ShareResponse
        {
            Instrument = new Share
            {
                Ticker = "AAPL",
                Currency = "USD",
                Sector = "Tech",
            },
        };

        var operationsMock = new Mock<OperationsService.OperationsServiceClient>();
        operationsMock
            .Setup(x => x.GetPortfolioAsync(It.IsAny<PortfolioRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(() => new AsyncUnaryCall<PortfolioResponse>(
                Task.FromResult(portfolioResponse),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        var instrumentsMock = new Mock<InstrumentsService.InstrumentsServiceClient>();
        instrumentsMock
            .Setup(x => x.ShareBy(It.IsAny<InstrumentRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(instrumentResponse);

        var client = new TinkoffPortfolioClient(
            Options.Create(new TinkoffClientConfiguration { AccountId = "123" }),
            new Mock<ILoggerService>().Object,
            MockApiClient(operations: operationsMock.Object, instruments: instrumentsMock.Object));

        var result = await client.GetPortfolioAsync();

        result.Should().HaveCount(1);
        result.First().Ticker.Symbol.Should().Be("AAPL");
        result.First().CurrentValue.Should().Be(10);
    }

    /// <summary>
    ///     Тест 5: PlaceMarketOrderAsync отправляет заявку и возвращает цену исполнения
    /// </summary>
    [Fact(DisplayName = "Тест 5: PlaceMarketOrderAsync отправляет заявку и возвращает цену исполнения")]
    public async Task PlaceMarketOrderAsync_ShouldReturnPrice()
    {
        var ordersMock = new Mock<OrdersService.OrdersServiceClient>();
        ordersMock
            .Setup(x => x.PostOrderAsync(It.IsAny<PostOrderRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Returns(() => new AsyncUnaryCall<PostOrderResponse>(
                Task.FromResult(new PostOrderResponse { ExecutedOrderPrice = new MoneyValue { Units = 99, Nano = 0 } }),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { }));

        var client = new TinkoffTradingClient(
            Options.Create(new TinkoffClientConfiguration { AccountId = "123" }),
            new Mock<ILoggerService>().Object,
            MockApiClient(orders: ordersMock.Object));

        var price = await client.PlaceMarketOrderAsync(new Ticker { Figi = "figi" }, 1, TradeAction.Buy);

        price.Should().Be(99);
    }

    /// <summary>
    ///     Тест 6: PlaceMarketOrderAsync должен выбросить ArgumentOutOfRangeException при неверном направлении
    /// </summary>
    [Fact(DisplayName = "Тест 6: PlaceMarketOrderAsync должен выбросить ArgumentOutOfRangeException при неверном направлении")]
    public async Task PlaceMarketOrderAsync_ShouldThrowArgumentOutOfRangeException()
    {
        var client = new TinkoffTradingClient(
            Options.Create(new TinkoffClientConfiguration { AccountId = "123" }),
            new Mock<ILoggerService>().Object,
            MockApiClient());

        var act = async () => await client.PlaceMarketOrderAsync(new Ticker { Figi = "figi" }, 1, (TradeAction)999);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    /// <summary>
    ///     Тест 7: GetHistoricalDataAsync должен выбросить исключение при ошибке GRPC
    /// </summary>
    [Fact(DisplayName = "Тест 7: GetHistoricalDataAsync должен выбросить исключение при ошибке GRPC")]
    public async Task GetHistoricalDataAsync_ShouldThrowException_WhenGrpcFails()
    {
        var marketDataMock = new Mock<MarketDataService.MarketDataServiceClient>();
        marketDataMock
            .Setup(x => x.GetCandlesAsync(It.IsAny<GetCandlesRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Throws(new RpcException(new Status(StatusCode.Internal, "ошибка свечей")));

        var loggerMock = new Mock<ILoggerService>();

        var client = new TinkoffMarketDataClient(
            loggerMock.Object,
            MockApiClient(marketData: marketDataMock.Object));

        var act = async () => await client.GetHistoricalDataAsync(new Ticker { Figi = "figi" }, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        await act.Should().ThrowAsync<TinkoffApiException>();
    }

    /// <summary>
    ///     Тест 8: GetPortfolioAsync должен выбросить исключение при ошибке GRPC
    /// </summary>
    [Fact(DisplayName = "Тест 8: GetPortfolioAsync должен выбросить исключение при ошибке GRPC")]
    public async Task GetPortfolioAsync_ShouldThrowException_WhenGrpcFails()
    {
        var operationsMock = new Mock<OperationsService.OperationsServiceClient>();
        operationsMock
            .Setup(x => x.GetPortfolioAsync(It.IsAny<PortfolioRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Throws(new RpcException(new Status(StatusCode.Unavailable, "портфель умер")));

        var client = new TinkoffPortfolioClient(
            Options.Create(new TinkoffClientConfiguration { AccountId = "123" }),
            new Mock<ILoggerService>().Object,
            MockApiClient(operations: operationsMock.Object));

        var act = async () => await client.GetPortfolioAsync();

        await act.Should().ThrowAsync<TinkoffApiException>();
    }

    /// <summary>
    ///     Тест 9: PlaceMarketOrderAsync должен выбросить исключение при ошибке GRPC
    /// </summary>
    [Fact(DisplayName = "Тест 9: PlaceMarketOrderAsync должен выбросить исключение при ошибке GRPC")]
    public async Task PlaceMarketOrderAsync_ShouldThrowException_WhenGrpcFails()
    {
        var ordersMock = new Mock<OrdersService.OrdersServiceClient>();
        ordersMock
            .Setup(x => x.PostOrderAsync(It.IsAny<PostOrderRequest>(), null, null, It.IsAny<CancellationToken>()))
            .Throws(new RpcException(new Status(StatusCode.Internal, "заказ пошёл по бороде")));

        var client = new TinkoffTradingClient(
            Options.Create(new TinkoffClientConfiguration { AccountId = "123" }),
            new Mock<ILoggerService>().Object,
            MockApiClient(orders: ordersMock.Object));

        var act = async () => await client.PlaceMarketOrderAsync(new Ticker { Figi = "figi" }, 1, TradeAction.Sell);

        await act.Should().ThrowAsync<TinkoffApiException>();
    }

    private static ITinkoffInvestApiClientWrapper MockApiClient(
        OrdersService.OrdersServiceClient? orders = null,
        MarketDataService.MarketDataServiceClient? marketData = null,
        InstrumentsService.InstrumentsServiceClient? instruments = null,
        OperationsService.OperationsServiceClient? operations = null)
    {
        var apiClient = new Mock<ITinkoffInvestApiClientWrapper>();
        apiClient.SetupGet(x => x.Orders).Returns(orders ?? Mock.Of<OrdersService.OrdersServiceClient>());
        apiClient.SetupGet(x => x.MarketData).Returns(marketData ?? Mock.Of<MarketDataService.MarketDataServiceClient>());
        apiClient.SetupGet(x => x.Instruments).Returns(instruments ?? Mock.Of<InstrumentsService.InstrumentsServiceClient>());
        apiClient.SetupGet(x => x.Operations).Returns(operations ?? Mock.Of<OperationsService.OperationsServiceClient>());
        return apiClient.Object;
    }
}
