using BlackCandle.API.Controllers;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace BlackCandle.Tests.API.Controllers;

/// <summary>
///     Тесты на <see cref="SettingsController" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Возвращает настройки при успешном получении</item>
///         <item>Возвращает ошибку при исключении в GetAsync</item>
///         <item>Вызывает сохранение и возвращает 200</item>
///     </list>
/// </remarks>
public sealed class SettingsControllerTests
{
    private readonly Mock<IBotSettingsService> _serviceMock = new();
    private readonly SettingsController _controller;

    /// <inheritdoc cref="SettingsControllerTests" />
    public SettingsControllerTests()
    {
        _controller = new SettingsController(_serviceMock.Object);
    }

    /// <summary>
    ///     Тест 1: Возвращает 200 и настройки при успешном получении
    /// </summary>
    [Fact(DisplayName = "Тест 1: Возвращает 200 и настройки при успешном получении")]
    public async Task Get_ShouldReturnSettings_WhenSuccessful()
    {
        // Arrange
        var expected = new BotSettings { EnableAutoTrading = true };
        _serviceMock.Setup(x => x.GetAsync()).ReturnsAsync(expected);

        // Act
        var result = await _controller.Get();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var operation = ((OkObjectResult)result.Result!).Value as OperationResult<BotSettings>;
        operation.Should().NotBeNull();
        operation!.IsSuccess.Should().BeTrue();
        operation.Data.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    ///     Тест 2: Возвращает 400 при исключении в GetAsync
    /// </summary>
    [Fact(DisplayName = "Тест 2: Возвращает 400 при исключении в GetAsync")]
    public async Task Get_ShouldReturnBadRequest_WhenExceptionThrown()
    {
        // Arrange
        _serviceMock.Setup(x => x.GetAsync()).ThrowsAsync(new Exception("fail"));

        // Act
        var result = await _controller.Get();

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var operation = ((BadRequestObjectResult)result.Result!).Value as OperationResult<BotSettings>;
        operation!.IsSuccess.Should().BeFalse();
        operation.Error.Should().Be("fail");
    }

    /// <summary>
    ///     Тест 3: Сохраняет настройки и возвращает 200
    /// </summary>
    [Fact(DisplayName = "Тест 3: Сохраняет настройки и возвращает 200")]
    public async Task Save_ShouldCallService_AndReturnOk()
    {
        // Arrange
        var input = new BotSettings { EnableAutoTrading = true };

        // Act
        var result = await _controller.Save(input);

        // Assert
        _serviceMock.Verify(x => x.SaveAsync(input), Times.Once);
        result.Should().BeOfType<OkResult>();
    }
}
