using BlackCandle.API.Controllers;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace BlackCandle.Tests.API.Controllers;

/// <summary>
///     Тесты на <see cref="TradeController"/>
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Успешный предпросмотр сделок</item>
///         <item>Ошибка при предпросмотре сделок</item>
///         <item>Успешный запуск торговли</item>
///         <item>Ошибка при запуске торговли</item>
///     </list>
/// </remarks>
public sealed class TradeControllerTests
{
    private readonly Mock<IUseCase<IReadOnlyCollection<OrderPreview>>> _previewMock = new();
    private readonly Mock<IUseCase<string>> _runTradeMock = new();

    private readonly TradeController _controller;

    /// <inheritdoc cref="TradeControllerTests" />
    public TradeControllerTests()
    {
        _controller = new TradeController(_previewMock.Object, _runTradeMock.Object);
    }

    /// <summary>
    ///     Тест 1: Должен вернуть 200 и список ордеров при успешном предпросмотре
    /// </summary>
    [Fact(DisplayName = "Тест 1: Должен вернуть 200 и список ордеров при успешном предпросмотре")]
    public async Task Preview_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var orders = new List<OrderPreview> { new(new Ticker { Symbol = string.Empty }, TradeAction.Buy, 1, 1) };
        var result = OperationResult<IReadOnlyCollection<OrderPreview>>.Success(orders);
        _previewMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Preview(default);

        // Assert
        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }

    /// <summary>
    ///     Тест 2: Должен вернуть 400 при ошибке предпросмотра
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен вернуть 400 при ошибке предпросмотра")]
    public async Task Preview_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var result = OperationResult<IReadOnlyCollection<OrderPreview>>.Failure("Ошибка предпросмотра");
        _previewMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Preview(default);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }

    /// <summary>
    ///     Тест 3: Должен вернуть 200 и строку при успешной торговле
    /// </summary>
    [Fact(DisplayName = "Тест 3: Должен вернуть 200 и строку при успешной торговле")]
    public async Task RunTrade_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var result = OperationResult<string>.Success("ok");
        _runTradeMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RunTrade(default);

        // Assert
        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }

    /// <summary>
    ///     Тест 4: Должен вернуть 400 при ошибке торговли
    /// </summary>
    [Fact(DisplayName = "Тест 4: Должен вернуть 400 при ошибке торговли")]
    public async Task RunTrade_ShouldReturnBadRequest_WhenFailed()
    {
        // Arrange
        var result = OperationResult<string>.Failure("Ошибка торговли");
        _runTradeMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RunTrade(default);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }
}
