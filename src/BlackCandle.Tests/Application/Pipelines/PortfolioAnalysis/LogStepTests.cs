using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Тесты на <see cref="LogStep" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Нет сигналов — сообщение содержит фразу "Нет торговых сигналов"</item>
///         <item>Формируется Markdown-отчёт с эмодзи и деталями</item>
///         <item>Вызывается Telegram-сервис</item>
///         <item>Сигналы фильтруются по дате</item>
///     </list>
/// </remarks>
public sealed class LogStepTests
{
    private readonly Mock<IRepository<TradeSignal>> _signalsRepo = new();
    private readonly Mock<ITelegramService> _telegram = new();
    private readonly Mock<IDataStorageContext> _storage = new();

    private readonly LogStep _step;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogStepTests"/> class.
    /// </summary>
    public LogStepTests()
    {
        _storage.Setup(x => x.TradeSignals).Returns(_signalsRepo.Object);
        _step = new LogStep(_telegram.Object, _storage.Object);
    }

    /// <summary>
    ///     Тест 1: Нет сигналов — сообщение содержит фразу "Нет торговых сигналов"
    /// </summary>
    [Fact(DisplayName = "Тест 1: Нет сигналов — сообщение содержит фразу 'Нет торговых сигналов'")]
    public async Task ExecuteAsync_ShouldSendEmptyMessage_WhenNoSignals()
    {
        // Arrange
        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<IFilter<TradeSignal>>()))
            .ReturnsAsync([]);

        var context = new PortfolioAnalysisContext { AnalysisTime = DateTime.Today.AddHours(10) };

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        _telegram.Verify(
            x => x.SendMessageAsync(It.Is<string>(s => s.Contains("Нет торговых сигналов")), It.IsAny<bool>()),
            Times.Once);
    }

    /// <summary>
    ///     Тест 2: Формируется Markdown-отчёт с эмодзи и деталями
    /// </summary>
    [Fact(DisplayName = "Тест 2: Формируется Markdown-отчёт с эмодзи и деталями")]
    public async Task ExecuteAsync_ShouldFormatSignals_WhenPresent()
    {
        // Arrange
        var signals = new List<TradeSignal>
        {
            new()
            {
                Ticker = new Ticker { Symbol = "AAPL" }, Action = TradeAction.Buy, Confidence = ConfidenceLevel.High,
                FundamentalScore = 5, Reason = "RSI < 30", Date = DateTime.Today,
            },
            new()
            {
                Ticker = new Ticker { Symbol = "SBER" }, Action = TradeAction.Sell, Confidence = ConfidenceLevel.Medium,
                FundamentalScore = 3, Reason = "ADX > 20", Date = DateTime.Today,
            },
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<IFilter<TradeSignal>>()))
            .ReturnsAsync(signals);

        var context = new PortfolioAnalysisContext { AnalysisTime = DateTime.Today.AddHours(15) };

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        _telegram.Verify(
            x => x.SendMessageAsync(It.Is<string>(msg => msg.Contains("Portfolio Analysis Report")), It.IsAny<bool>()),
            Times.Once);
    }

    /// <summary>
    ///     Тест 3: Telegram-сервис вызывается один раз
    /// </summary>
    [Fact(DisplayName = "Тест 3: Telegram-сервис вызывается один раз")]
    public async Task ExecuteAsync_ShouldCallTelegramService_Once()
    {
        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<IFilter<TradeSignal>>()))
            .ReturnsAsync([]);

        var context = new PortfolioAnalysisContext { AnalysisTime = DateTime.UtcNow };
        await _step.ExecuteAsync(context);

        _telegram.Verify(x => x.SendMessageAsync(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
    }

    /// <summary>
    ///     Тест 4: Сигналы фильтруются по дате
    /// </summary>
    [Fact(DisplayName = "Тест 4: Сигналы фильтруются по дате")]
    public async Task ExecuteAsync_ShouldOnlyIncludeTodaySignals()
    {
        // Arrange
        var today = DateTime.Now.Date;
        var signals = new List<TradeSignal>
        {
            new() { Ticker = new Ticker { Symbol = "AAPL" }, Date = today },
            new() { Ticker = new Ticker { Symbol = "SBER" }, Date = today.AddDays(-1) }, // не должен попасть
        };

        _signalsRepo.Setup(x => x.GetAllAsync(It.IsAny<IFilter<TradeSignal>>()))
            .ReturnsAsync((IFilter<TradeSignal> filter) => filter.Apply(signals.AsQueryable()).ToList());

        var context = new PortfolioAnalysisContext { AnalysisTime = DateTime.Now };

        // Act
        await _step.ExecuteAsync(context);

        // Assert
        _telegram.Verify(
            x => x.SendMessageAsync(
                It.Is<string>(msg => msg.Contains("AAPL") && !msg.Contains("SBER")),
                It.IsAny<bool>()), Times.Once);
    }
}
