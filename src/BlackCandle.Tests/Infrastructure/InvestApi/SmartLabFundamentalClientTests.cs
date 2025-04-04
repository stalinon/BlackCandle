using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Infrastructure.InvestApi.SmartLab;
using Moq;

namespace BlackCandle.Tests.Infrastructure.InvestApi;

/// <summary>
///     Тесты на <see cref="SmartLabFundamentalClient" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Тест на кэширование: если данные уже актуальны, не загружать заново.</item>
///         <item>Тест на исключение при парсинге данных.</item>
///         <item>Тест на корректную обработку парсинга данных (в том числе знаков и пробелов).</item>
///     </list>
/// </remarks>
public class SmartLabFundamentalClientTests
{
    private readonly Mock<IRepository<FundamentalData>> _repositoryMock;
    private readonly SmartLabFundamentalClient _client;

    public SmartLabFundamentalClientTests()
    {
        _repositoryMock = new Mock<IRepository<FundamentalData>>();
        var dataStorageMock = new Mock<IDataStorageContext>();
        dataStorageMock.Setup(x => x.Fundamentals).Returns(_repositoryMock.Object);
        
        Mock<ILoggerService> loggerMock = new();
        _client = new SmartLabFundamentalClient(dataStorageMock.Object, loggerMock.Object);
    }

    /// <summary>
    ///     Тест 1: Кэширование — если данные актуальны, не загружать заново.
    /// </summary>
    [Fact(DisplayName = "Тест 1: Кэширование — если данные актуальны, не загружать заново.")]
    public async Task GetFundamentalsAsync_ShouldReturnCachedData_WhenDataIsUpToDate()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL" };
        var cachedData = new FundamentalData { Ticker = "AAPL", LastUpdated = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetByIdAsync("AAPL")).ReturnsAsync(cachedData);

        // Act
        var result = await _client.GetFundamentalsAsync(ticker);

        // Assert
        Assert.Equal(cachedData, result);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FundamentalData>()), Times.Never);
    }

    /// <summary>
    ///     Тест 2: Корректная обработка парсинга данных с учетом знаков и пробелов.
    /// </summary>
    [Theory(DisplayName = "Тест 2: Корректная обработка парсинга данных с учетом знаков и пробелов.")]
    [InlineData("−5", -5.0)]
    [InlineData(" 5 ", 5.0)]
    [InlineData(" 50.25 ", 50.25)]
    public void ParseDecimal_ShouldHandleVariousFormats(string input, double? expected)
    {
        // Act
        var result = SmartLabFundamentalClient.ParseDecimal(input);

        // Assert
        Assert.Equal((decimal?)expected, result);
    }
}
