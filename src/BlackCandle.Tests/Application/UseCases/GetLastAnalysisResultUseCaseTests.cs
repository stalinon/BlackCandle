using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.UseCases;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

using FluentAssertions;

using Moq;

namespace BlackCandle.Tests.Application.UseCases;

/// <remarks>
///     Тесты для <see cref="GetLastAnalysisResultUseCase" />
/// <list type="number">
///     <item>Должен вернуть список сигналов, если он не пуст</item>
///     <item>Должен вернуть ошибку, если список сигналов пуст</item>
///     <item>Должен вернуть ошибку, если репозиторий выбрасывает исключение</item>
/// </list>
/// </remarks>
public sealed class GetLastAnalysisResultUseCaseTests
{
    private readonly Mock<IRepository<TradeSignal>> _signals = new();
    private readonly Mock<IDataStorageContext> _context = new();

    private readonly GetLastAnalysisResultUseCase _sut;

    /// <inheritdoc cref="GetLastAnalysisResultUseCaseTests" />
    public GetLastAnalysisResultUseCaseTests()
    {
        _context
            .Setup(x => x.TradeSignals)
            .Returns(_signals.Object);

        _sut = new GetLastAnalysisResultUseCase(_context.Object);
    }

    /// <summary>
    ///     Тест 1: Должен вернуть список сигналов, если он не пуст
    /// </summary>
    [Fact(DisplayName = "Тест 1: Должен вернуть список сигналов, если он не пуст")]
    public async Task Should_return_signals_when_present()
    {
        // Arrange
        var expected = new List<TradeSignal>()
        {
            new()
            {
                Ticker = new() { Symbol = "AAPL" },
                Action = TradeAction.Buy,
            },
            new()
            {
                Ticker = new() { Symbol = "NVDA" },
                Action = TradeAction.Sell,
            },
        };

        _signals
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>?>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _sut.ExecuteAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expected);

        _signals.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>?>()), Times.Once);
    }

    /// <summary>
    ///     Тест 2: Должен вернуть ошибку, если список сигналов пуст
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен вернуть ошибку, если список сигналов пуст")]
    public async Task Should_return_failure_when_signals_are_empty()
    {
        // Arrange
        _signals
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>?>()))
            .ReturnsAsync([]);

        // Act
        var result = await _sut.ExecuteAsync();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Нет сигналов анализа");

        _signals.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>?>()), Times.Once);
    }

    /// <summary>
    ///     Тест 3: Должен вернуть ошибку, если репозиторий выбрасывает исключение
    /// </summary>
    [Fact(DisplayName = "Тест 3: Должен вернуть ошибку, если репозиторий выбрасывает исключение")]
    public async Task Should_return_failure_when_repository_throws()
    {
        // Arrange
        _signals
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>?>()))
            .ThrowsAsync(new Exception("storage is dead"));

        // Act
        var result = await _sut.ExecuteAsync();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("storage is dead");

        _signals.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<TradeSignal, bool>>?>()), Times.Once);
    }
}
