using System.Linq.Expressions;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using Moq;

namespace BlackCandle.Tests.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Тесты на <see cref="ValidateTradeLimitsStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item><description>Валидный сигнал — остаётся в списке</description></item>
///         <item><description>Невалидный сигнал — отфильтровывается</description></item>
///         <item><description>Все сигналы отклонены — список пуст</description></item>
///         <item><description>Портфель получается из хранилища</description></item>
///     </list>
/// </remarks>
public sealed class ValidateTradeLimitsStepTests
{
    private readonly Mock<ITradeLimitValidator> _validator = new();
    private readonly Mock<IRepository<PortfolioAsset>> _portfolioRepo = new();
    private readonly Mock<IDataStorageContext> _storage = new();

    private readonly ValidateTradeLimitsStep _step;

    public ValidateTradeLimitsStepTests()
    {
        _storage.Setup(x => x.PortfolioAssets).Returns(_portfolioRepo.Object);
        _step = new ValidateTradeLimitsStep(_storage.Object, _validator.Object);
    }

    private AutoTradeExecutionContext ContextWithSignals(params TradeSignal[] signals) =>
        new() { Signals = signals.ToList() };

    private static List<PortfolioAsset> MockPortfolio() => new()
    {
        new PortfolioAsset
        {
            Ticker = new Ticker { Symbol = "AAPL" },
            Quantity = 5,
            CurrentValue = 10000
        }
    };

    /// <summary>
    ///     Тест 1: Валидный сигнал — остаётся в списке
    /// </summary>
    [Fact(DisplayName = "Тест 1: Валидный сигнал — остаётся в списке")]
    public async Task ExecuteAsync_ShouldKeepValidSignals()
    {
        // Arrange
        var signal = new TradeSignal { Ticker = new Ticker { Symbol = "AAPL" }, Action = TradeAction.Buy };
        var context = ContextWithSignals(signal);

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync(MockPortfolio());

        _validator.Setup(x => x.Validate(signal, It.IsAny<List<PortfolioAsset>>()))
            .Returns(true);

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        Assert.Single(context.Signals);
        Assert.Equal("AAPL", context.Signals[0].Ticker.Symbol);
    }

    /// <summary>
    ///     Тест 2: Невалидный сигнал — отфильтровывается
    /// </summary>
    [Fact(DisplayName = "Тест 2: Невалидный сигнал — отфильтровывается")]
    public async Task ExecuteAsync_ShouldRemoveInvalidSignals()
    {
        var signal = new TradeSignal { Ticker = new Ticker { Symbol = "AAPL" }, Action = TradeAction.Buy };
        var context = ContextWithSignals(signal);

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync(MockPortfolio());

        _validator.Setup(x => x.Validate(It.IsAny<TradeSignal>(), It.IsAny<List<PortfolioAsset>>()))
            .Returns(false);

        await _step.ExecuteAsync(context);

        Assert.Empty(context.Signals);
    }

    /// <summary>
    ///     Тест 3: Все сигналы отклонены — список пуст
    /// </summary>
    [Fact(DisplayName = "Тест 3: Все сигналы отклонены — список пуст")]
    public async Task ExecuteAsync_ShouldReturnEmpty_WhenAllInvalid()
    {
        var signals = new[]
        {
            new TradeSignal { Ticker = new Ticker { Symbol = "AAPL" } },
            new TradeSignal { Ticker = new Ticker { Symbol = "SBER" } }
        };

        var context = ContextWithSignals(signals);

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync(MockPortfolio());

        _validator.Setup(x => x.Validate(It.IsAny<TradeSignal>(), It.IsAny<List<PortfolioAsset>>()))
            .Returns(false);

        await _step.ExecuteAsync(context);

        Assert.Empty(context.Signals);
    }

    /// <summary>
    ///     Тест 4: Портфель получается из хранилища
    /// </summary>
    [Fact(DisplayName = "Тест 4: Портфель получается из хранилища")]
    public async Task ExecuteAsync_ShouldCallPortfolioRepo()
    {
        var signal = new TradeSignal { Ticker = new Ticker { Symbol = "AAPL" } };
        var context = ContextWithSignals(signal);

        _portfolioRepo.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync(MockPortfolio());

        _validator.Setup(x => x.Validate(It.IsAny<TradeSignal>(), It.IsAny<List<PortfolioAsset>>()))
            .Returns(true);

        await _step.ExecuteAsync(context);

        _portfolioRepo.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()), Times.Once);
    }
}
