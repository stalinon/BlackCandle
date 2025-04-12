using AngleSharp.Html.Parser;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Infrastructure.InvestApi.SmartLab;
using BlackCandle.Tests.Models;

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
///         <item>Тест на ошибку при загрузке кидает SmartLabScrapingException и логирует ошибку.</item>
///     </list>
/// </remarks>
public class SmartLabFundamentalClientTests
{
    private readonly Mock<IRepository<FundamentalData>> _repositoryMock;
    private readonly SmartLabFundamentalClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartLabFundamentalClientTests"/> class.
    /// </summary>
    public SmartLabFundamentalClientTests()
    {
        _repositoryMock = new Mock<IRepository<FundamentalData>>();
        Mock<ILoggerService> loggerMock = new();
        _client = new SmartLabFundamentalClient(_repositoryMock.Object, loggerMock.Object);
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

    /// <summary>
    ///     Тест 3: Парсинг HTML — корректный разбор таблицы из SmartLab.
    /// </summary>
    [Fact(DisplayName = "Тест 3: Парсинг HTML — корректный разбор таблицы из SmartLab")]
    public async Task LoadFundamentalsTableAsync_ShouldParseHtmlCorrectly()
    {
        // Arrange
        var html = await File.ReadAllTextAsync("Data/smartlab.html"); // путь к реальному HTML
        var parser = new HtmlParser();
        var doc = await parser.ParseDocumentAsync(html);

        var client = new SmartLabFundamentalClientTestWrapper(_repositoryMock.Object, new Mock<ILoggerService>().Object);

        // Act
        var table = await client.TestParseHtml(doc); // доступ через обёртку

        // Assert
        Assert.NotEmpty(table);
        Assert.Contains("GAZP", table.Keys);
        Assert.True(table["GAZP"].PERatio.HasValue);
    }

    /// <summary>
    ///     Тест 4: При ошибке загрузки кидает SmartLabScrapingException и логирует ошибку
    /// </summary>
    [Fact(DisplayName = "Тест 4: При ошибке загрузки кидает SmartLabScrapingException и логирует ошибку")]
    public async Task LoadFundamentalsTableAsync_ShouldThrowAndLog_WhenHttpFails()
    {
        // Arrange
        var repo = new Mock<IRepository<FundamentalData>>();
        var logger = new Mock<ILoggerService>();

        var client = new SmartLabFundamentalClientTestWrapper(repo.Object, logger.Object)
        {
            SimulateFailure = true,
        };

        // Act + Assert
        var ex = await Assert.ThrowsAsync<SmartLabScrapingException>(() =>
            client.TestGetFundamentalsAsync(new Ticker { Symbol = "GAZP" }));

        Assert.Contains("SmartLab", ex.Message);
        logger.Verify(x => x.LogError("SmartLab: ошибка парсинга фундаментала", It.IsAny<Exception>()), Times.Once);
    }
}
