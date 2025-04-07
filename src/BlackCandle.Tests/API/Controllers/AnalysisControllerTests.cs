using BlackCandle.API.Controllers;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace BlackCandle.Tests.API.Controllers;

/// <summary>
///     Тесты на <see cref="AnalysisController"/>
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Успешное получение сигналов</item>
///         <item>Ошибка при получении сигналов</item>
///         <item>Успешный запуск анализа</item>
///         <item>Ошибка при запуске анализа</item>
///     </list>
/// </remarks>
public sealed class AnalysisControllerTests
{
    private readonly Mock<IUseCase<IReadOnlyCollection<TradeSignal>>> _getSignalsMock = new();
    private readonly Mock<IUseCase<string>> _runAnalysisMock = new();

    private readonly AnalysisController _controller;

    /// <inheritdoc cref="AnalysisControllerTests" />
    public AnalysisControllerTests()
    {
        _controller = new AnalysisController(_getSignalsMock.Object, _runAnalysisMock.Object);
    }

    /// <summary>
    ///     Тест 1: Должен вернуть 200 и список сигналов при успешном выполнении
    /// </summary>
    [Fact(DisplayName = "Тест 1: Должен вернуть 200 и список сигналов при успешном выполнении")]
    public async Task GetSignals_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var signals = new List<TradeSignal> { new() };
        var result = OperationResult<IReadOnlyCollection<TradeSignal>>.Success(signals);
        _getSignalsMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetSignals(default);

        // Assert
        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }

    /// <summary>
    ///     Тест 2: Должен вернуть 400 при ошибке получения сигналов
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен вернуть 400 при ошибке получения сигналов")]
    public async Task GetSignals_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var result = OperationResult<IReadOnlyCollection<TradeSignal>>.Failure("Ошибка");
        _getSignalsMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetSignals(default);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }

    /// <summary>
    ///     Тест 3: Должен вернуть 200 и строку при успешном запуске анализа
    /// </summary>
    [Fact(DisplayName = "Тест 3: Должен вернуть 200 и строку при успешном запуске анализа")]
    public async Task RunAnalysis_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var result = OperationResult<string>.Success("done");
        _runAnalysisMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RunAnalysis(default);

        // Assert
        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }

    /// <summary>
    ///     Тест 4: Должен вернуть 400 при ошибке анализа
    /// </summary>
    [Fact(DisplayName = "Тест 4: Должен вернуть 400 при ошибке анализа")]
    public async Task RunAnalysis_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var result = OperationResult<string>.Failure("Ошибка анализа");
        _runAnalysisMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RunAnalysis(default);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }
}
