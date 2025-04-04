using System.Linq.Expressions;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using Moq;

namespace BlackCandle.Tests.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Тесты на <see cref="UpdatePortfolioStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Нет успешных сделок — ничего не происходит</item>
///         <item>Buy создаёт новый актив</item>
///         <item>Buy обновляет текущий актив</item>
///         <item>Sell уменьшает количество</item>
///         <item>Sell обнуляет актив — он удаляется</item>
///         <item>ExecutedTrades сохраняются</item>
///     </list>
/// </remarks>
public sealed class UpdatePortfolioStepTests
{
    private readonly Mock<IRepository<PortfolioAsset>> _portfolioRepo = new();
    private readonly Mock<IRepository<ExecutedTrade>> _executedRepo = new();
    private readonly Mock<IDataStorageContext> _storage = new();

    private readonly UpdatePortfolioStep _step;

    public UpdatePortfolioStepTests()
    {
        _storage.Setup(x => x.PortfolioAssets).Returns(_portfolioRepo.Object);
        _storage.Setup(x => x.ExecutedTrades).Returns(_executedRepo.Object);

        _step = new UpdatePortfolioStep(_storage.Object);
    }

    /// <summary>
    ///     Тест 1: Нет успешных сделок — ничего не происходит
    /// </summary>
    [Fact(DisplayName = "Тест 1: Нет успешных сделок — ничего не происходит")]
    public async Task ExecuteAsync_ShouldDoNothing_WhenNoSuccess()
    {
        var context = new AutoTradeExecutionContext
        {
            ExecutedTrades = [new() { Status = TradeStatus.Error }]
        };

        await _step.ExecuteAsync(context);

        _portfolioRepo.Verify(x => x.AddAsync(It.IsAny<PortfolioAsset>()), Times.Never);
        _executedRepo.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<ExecutedTrade>>()), Times.Never);
    }

    /// <summary>
    ///     Тест 2: Buy создаёт новый актив
    /// </summary>
    [Fact(DisplayName = "Тест 2: Buy создаёт новый актив")]
    public async Task ExecuteAsync_ShouldCreateAsset_WhenNotExists()
    {
        var ticker = new Ticker { Symbol = "AAPL" };

        var trade = new ExecutedTrade
        {
            Ticker = ticker,
            Side = TradeAction.Buy,
            Quantity = 10,
            Price = 100,
            Status = TradeStatus.Success
        };

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync(new List<PortfolioAsset>());

        var context = new AutoTradeExecutionContext { ExecutedTrades = [trade] };

        await _step.ExecuteAsync(context);

        _portfolioRepo.Verify(x => x.AddAsync(It.Is<PortfolioAsset>(a =>
            a.Ticker.Symbol == "AAPL" && a.Quantity == 10 && a.CurrentValue == 100)), Times.Once);
    }

    /// <summary>
    ///     Тест 3: Buy обновляет текущий актив
    /// </summary>
    [Fact(DisplayName = "Тест 3: Buy обновляет текущий актив")]
    public async Task ExecuteAsync_ShouldUpdateAsset_WhenExists()
    {
        var ticker = new Ticker { Symbol = "AAPL" };

        var existing = new PortfolioAsset
        {
            Ticker = ticker,
            Quantity = 10,
            CurrentValue = 100
        };

        var trade = new ExecutedTrade
        {
            Ticker = ticker,
            Side = TradeAction.Buy,
            Quantity = 10,
            Price = 200,
            Status = TradeStatus.Success
        };

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync([existing]);

        var context = new AutoTradeExecutionContext { ExecutedTrades = [trade] };

        await _step.ExecuteAsync(context);

        _portfolioRepo.Verify(x => x.AddAsync(It.Is<PortfolioAsset>(a =>
            a.Quantity == 20 &&
            a.CurrentValue == 150 // (100*10 + 200*10) / 20
        )), Times.Once);
    }

    /// <summary>
    ///     Тест 4: Sell уменьшает количество
    /// </summary>
    [Fact(DisplayName = "Тест 4: Sell уменьшает количество")]
    public async Task ExecuteAsync_ShouldReduceQuantity_WhenSelling()
    {
        var ticker = new Ticker { Symbol = "AAPL" };

        var existing = new PortfolioAsset
        {
            Ticker = ticker,
            Quantity = 10,
            CurrentValue = 100
        };

        var trade = new ExecutedTrade
        {
            Ticker = ticker,
            Side = TradeAction.Sell,
            Quantity = 4,
            Price = 0,
            Status = TradeStatus.Success
        };

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync([existing]);

        var context = new AutoTradeExecutionContext { ExecutedTrades = [trade] };

        await _step.ExecuteAsync(context);

        _portfolioRepo.Verify(x => x.AddAsync(It.Is<PortfolioAsset>(a => a.Quantity == 6)), Times.Once);
    }

    /// <summary>
    ///     Тест 5: Sell обнуляет актив — он удаляется
    /// </summary>
    [Fact(DisplayName = "Тест 5: Sell обнуляет актив — он удаляется")]
    public async Task ExecuteAsync_ShouldRemoveAsset_WhenQuantityBecomesZero()
    {
        var ticker = new Ticker { Symbol = "AAPL" };

        var existing = new PortfolioAsset
        {
            Ticker = ticker,
            Quantity = 5,
            CurrentValue = 100
        };

        var trade = new ExecutedTrade
        {
            Ticker = ticker,
            Side = TradeAction.Sell,
            Quantity = 5,
            Price = 0,
            Status = TradeStatus.Success
        };

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync([existing]);

        var context = new AutoTradeExecutionContext { ExecutedTrades = [trade] };

        await _step.ExecuteAsync(context);

        _portfolioRepo.Verify(x => x.RemoveAsync(existing.Id), Times.Once);
    }

    /// <summary>
    ///     Тест 6: ExecutedTrades сохраняются
    /// </summary>
    [Fact(DisplayName = "Тест 6: ExecutedTrades сохраняются")]
    public async Task ExecuteAsync_ShouldSaveExecutedTrades()
    {
        var trade = new ExecutedTrade
        {
            Ticker = new Ticker { Symbol = "AAPL" },
            Side = TradeAction.Buy,
            Quantity = 1,
            Price = 100,
            Status = TradeStatus.Success
        };

        var context = new AutoTradeExecutionContext { ExecutedTrades = [trade] };

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>())).ReturnsAsync([]);

        await _step.ExecuteAsync(context);

        _executedRepo.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<ExecutedTrade>>(col => col.Contains(trade))), Times.Once);
    }
}
