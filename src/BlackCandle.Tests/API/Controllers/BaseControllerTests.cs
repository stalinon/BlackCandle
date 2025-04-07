using BlackCandle.API.Controllers;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.Tests.API.Controllers;

/// <summary>
///     Тесты на <see cref="BaseController"/>
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Успешный ответ оборачивается в OperationResult</item>
///         <item>Ошибка оборачивается в OperationResult с 400</item>
///     </list>
/// </remarks>
public sealed class BaseControllerTests
{
    private readonly TestableController _controller = new();

    /// <inheritdoc cref="BaseControllerTests" />
    public BaseControllerTests() { }

    /// <summary>
    ///     Тест 1: Успешный ответ оборачивается в OperationResult
    /// </summary>
    [Fact(DisplayName = "Тест 1: Успешный ответ оборачивается в OperationResult")]
    public void Success_ShouldReturnOkWithWrappedResult()
    {
        // Arrange
        const string data = "OK";

        // Act
        var result = _controller.CallSuccess(data);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(OperationResult<string>.Success(data));
    }

    /// <summary>
    ///     Тест 2: Ошибка оборачивается в OperationResult с 400
    /// </summary>
    [Fact(DisplayName = "Тест 2: Ошибка оборачивается в OperationResult с 400")]
    public void Fail_ShouldReturnBadRequestWithWrappedError()
    {
        // Arrange
        const string error = "Ошибка";

        // Act
        var result = _controller.CallFail<string>(error);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().BeEquivalentTo(OperationResult<string>.Failure(error));
    }

    private sealed class TestableController : BaseController
    {
        public ActionResult<OperationResult<T>> CallSuccess<T>(T value) => Success(value);

        public ActionResult<OperationResult<T>> CallFail<T>(string error) => Fail<T>(error);
    }
}
