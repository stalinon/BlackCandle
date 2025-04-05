using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="LoadPortfolioStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Получение портфеля из InvestAPI</item>
///         <item>Очистка старых активов</item>
///         <item>Сохранение новых активов</item>
///     </list>
/// </remarks>
public sealed class LoadPortfolioStepTests
{
    /// <summary>
    ///     Тест 1: Получение портфеля из InvestAPI
    /// </summary>
    [Fact(DisplayName = "Тест 1: Получение портфеля из InvestAPI")]
    public async Task ExecuteAsync_ShouldLoadPortfolioFromApi()
    {
        // Arrange
        var mockPortfolioClient = new Mock<IPortfolioClient>();
        var mockFacade = new Mock<IInvestApiFacade>();
        var mockStorage = new Mock<IDataStorageContext>();
        var portfolioRepo = new Mock<IRepository<PortfolioAsset>>();

        var expected = new List<PortfolioAsset>
        {
            new() { Ticker = new Ticker { Symbol = "AAPL" }, Quantity = 10 },
            new() { Ticker = new Ticker { Symbol = "SBER" }, Quantity = 5 },
        };

        mockPortfolioClient.Setup(x => x.GetPortfolioAsync()).ReturnsAsync(expected);
        mockFacade.Setup(x => x.Portfolio).Returns(mockPortfolioClient.Object);
        mockStorage.Setup(x => x.PortfolioAssets).Returns(portfolioRepo.Object);

        var step = new LoadPortfolioStep(mockFacade.Object, mockStorage.Object);
        var context = new PortfolioAnalysisContext();

        // Act
        await step.ExecuteAsync(context);

        // Assert
        mockPortfolioClient.Verify(x => x.GetPortfolioAsync(), Times.Once);
        portfolioRepo.Verify(x => x.TruncateAsync(), Times.Once);
        portfolioRepo.Verify(x => x.AddRangeAsync(expected), Times.Once);
    }
}
