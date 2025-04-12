using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;

using FluentAssertions;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="DiscoverNewTickersStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Вызывает GetTopTickersAsync с правильным количеством</item>
///         <item>Добавляет тикеры в контекст</item>
///     </list>
/// </remarks>
public sealed class DiscoverNewTickersStepTests
{
    private readonly Mock<IInstrumentClient> _instrumentMock = new();
    private readonly Mock<IInvestApiFacade> _investMock = new();

    private readonly DiscoverNewTickersStep _step;
    private readonly PortfolioAnalysisContext _context = new();

    /// <inheritdoc cref="DiscoverNewTickersStepTests"/>
    public DiscoverNewTickersStepTests()
    {
        var tickers = new List<Ticker>
        {
            new() { Symbol = "AAPL" },
            new() { Symbol = "SBER" },
        };

        _instrumentMock
            .Setup(x => x.GetTopTickersAsync(It.IsAny<int>()))
            .ReturnsAsync(tickers);

        _investMock.Setup(x => x.Instruments).Returns(_instrumentMock.Object);

        _step = new DiscoverNewTickersStep(_investMock.Object);
    }

    /// <summary>
    ///     Тест 1: Вызывает GetTopTickersAsync с правильным количеством
    /// </summary>
    [Fact(DisplayName = "Тест 1: Вызывает GetTopTickersAsync с правильным количеством")]
    public async Task ExecuteAsync_ShouldCallGetTopTickers_WithMaxLimit()
    {
        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _instrumentMock.Verify(x => x.GetTopTickersAsync(100), Times.Once);
    }

    /// <summary>
    ///     Тест 2: Добавляет тикеры в контекст
    /// </summary>
    [Fact(DisplayName = "Тест 2: Добавляет тикеры в контекст")]
    public async Task ExecuteAsync_ShouldAddTickersToContext()
    {
        // Act
        await _step.ExecuteAsync(_context);

        // Assert
        _context.Tickers.Should().Contain(t => t.Symbol == "AAPL");
        _context.Tickers.Should().Contain(t => t.Symbol == "SBER");
    }
}
