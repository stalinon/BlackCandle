using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Infrastructure.InvestApi;
using Moq;

namespace BlackCandle.Tests.Infrastructure.InvestApi;

/// <summary>
///     Тесты на <see cref="InvestApiFacade" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Проверка, что все сервисы передаются и возвращаются корректно</item>
///     </list>
/// </remarks>
public sealed class InvestApiFacadeTests
{
    /// <summary>
    ///     Тест 1: Проверка, что все сервисы передаются и возвращаются корректно
    /// </summary>
    [Fact(DisplayName = "Тест 1: Проверка, что все сервисы передаются и возвращаются корректно")]
    public void Facade_ShouldExposeAllClientsCorrectly()
    {
        // Arrange
        var market = new Mock<IMarketDataClient>().Object;
        var portfolio = new Mock<IPortfolioClient>().Object;
        var trading = new Mock<ITradingClient>().Object;
        var fundamentals = new Mock<IFundamentalDataClient>().Object;

        // Act
        var facade = new InvestApiFacade(market, portfolio, trading, fundamentals);

        // Assert
        Assert.Equal(market, facade.Marketdata);
        Assert.Equal(portfolio, facade.Portfolio);
        Assert.Equal(trading, facade.Trading);
        Assert.Equal(fundamentals, facade.Fundamentals);
    }
}