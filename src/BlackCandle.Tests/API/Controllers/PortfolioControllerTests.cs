using BlackCandle.API.Controllers;
using BlackCandle.Application.UseCases.Abstractions;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace BlackCandle.Tests.API.Controllers;

/// <summary>
///     Тесты на <see cref="PortfolioController"/>
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Успешное получение портфеля</item>
///         <item>Ошибка при получении портфеля</item>
///     </list>
/// </remarks>
public sealed class PortfolioControllerTests
{
    private readonly Mock<IUseCase<IReadOnlyCollection<PortfolioAsset>>> _useCaseMock = new();

    private readonly PortfolioController _controller;

    /// <inheritdoc cref="PortfolioControllerTests" />
    public PortfolioControllerTests()
    {
        _controller = new PortfolioController(_useCaseMock.Object);
    }

    /// <summary>
    ///     Тест 1: Должен вернуть 200 и список активов при успешном выполнении
    /// </summary>
    [Fact(DisplayName = "Тест 1: Должен вернуть 200 и список активов при успешном выполнении")]
    public async Task Get_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var data = new List<PortfolioAsset> { new() };
        var result = OperationResult<IReadOnlyCollection<PortfolioAsset>>.Success(data);
        _useCaseMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Get(default);

        // Assert
        response.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }

    /// <summary>
    ///     Тест 2: Должен вернуть 404 при ошибке выполнения
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен вернуть 404 при ошибке выполнения")]
    public async Task Get_ShouldReturnNotFound_WhenFailed()
    {
        // Arrange
        var result = OperationResult<IReadOnlyCollection<PortfolioAsset>>.Failure("Ошибка");
        _useCaseMock.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Get(default);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(result);
    }
}
