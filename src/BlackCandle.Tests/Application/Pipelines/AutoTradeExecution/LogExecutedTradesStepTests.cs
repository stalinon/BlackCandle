using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Тесты на <see cref="LogExecutedTradesStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Нет сделок — Telegram не вызывается</item>
///         <item>Только неудачные — отчёт содержит ⚠️</item>
///         <item>Смешанные сделки — обе секции присутствуют</item>
///         <item>Формат отчета соответствует Markdown</item>
///     </list>
/// </remarks>
public sealed class LogExecutedTradesStepTests
{
    private readonly Mock<ITelegramService> _telegram = new();
    private readonly LogExecutedTradesStep _step;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogExecutedTradesStepTests"/> class.
    /// </summary>
    public LogExecutedTradesStepTests()
    {
        _step = new LogExecutedTradesStep(_telegram.Object);
    }

    /// <summary>
    ///     Тест 1: Нет сделок — Telegram не вызывается
    /// </summary>
    [Fact(DisplayName = "Тест 1: Нет сделок — Telegram не вызывается")]
    public async Task ExecuteAsync_ShouldNotSend_WhenNoTrades()
    {
        var context = new AutoTradeExecutionContext { ExecutedTrades = [] };

        await _step.ExecuteAsync(context);

        _telegram.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    /// <summary>
    ///     Тест 2: Только неудачные — отчёт содержит ⚠️
    /// </summary>
    [Fact(DisplayName = "Тест 2: Только неудачные — отчёт содержит ⚠️")]
    public async Task ExecuteAsync_ShouldSendFailureReport()
    {
        var context = new AutoTradeExecutionContext
        {
            ExecutedTrades = [MakeTrade("SBER", TradeAction.Sell, TradeStatus.Error)],
        };

        await _step.ExecuteAsync(context);

        _telegram.Verify(
            x => x.SendMessageAsync(
            It.Is<string>(s =>
            s.Contains("❌ *Ошибки исполнения:*") &&
            s.Contains("⚠️ `SBER`")), It.IsAny<bool>()), Times.Once);
    }

    /// <summary>
    ///     Тест 3: Смешанные сделки — обе секции присутствуют
    /// </summary>
    [Fact(DisplayName = "Тест 3: Смешанные сделки — обе секции присутствуют")]
    public async Task ExecuteAsync_ShouldIncludeBothSuccessAndFailure()
    {
        var context = new AutoTradeExecutionContext
        {
            ExecutedTrades =
            [
                MakeTrade("AAPL", TradeAction.Buy, TradeStatus.Success, 200),
                MakeTrade("YNDX", TradeAction.Sell, TradeStatus.Error)
            ],
        };

        await _step.ExecuteAsync(context);

        _telegram.Verify(
            x => x.SendMessageAsync(
            It.Is<string>(s =>
            s.Contains("✅ *Успешные сделки:*") &&
            s.Contains("❌ *Ошибки исполнения:*") &&
            s.Contains("🟢 `AAPL`") &&
            s.Contains("⚠️ `YNDX`")), It.IsAny<bool>()), Times.Once);
    }

    /// <summary>
    ///     Тест 4: Формат отчета соответствует Markdown
    /// </summary>
    [Fact(DisplayName = "Тест 4: Формат отчета соответствует Markdown")]
    public async Task ExecuteAsync_ShouldUseMarkdownFormat()
    {
        var context = new AutoTradeExecutionContext
        {
            ExecutedTrades = [MakeTrade("AAPL", TradeAction.Buy, TradeStatus.Success, 100)],
        };

        await _step.ExecuteAsync(context);

        _telegram.Verify(
            x => x.SendMessageAsync(
            It.Is<string>(s =>
                s.StartsWith("📦 *Auto-Trade Execution Report*") &&
                s.Contains("*Успешные сделки:*") &&
                s.Contains("`AAPL`") &&
                s.Contains('*')), // markdown
            It.IsAny<bool>()), Times.Once);
    }

    private static ExecutedTrade MakeTrade(string ticker, TradeAction side, TradeStatus status, decimal price = 0)
    {
        return new ExecutedTrade
        {
            Ticker = new Ticker { Symbol = ticker },
            Quantity = 10,
            Side = side,
            Status = status,
            Price = price,
        };
    }
}
