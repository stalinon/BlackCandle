using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.UseCases;
using BlackCandle.Domain.Entities;

using FluentAssertions;

using Moq;

namespace BlackCandle.Tests.Application.UseCases;

/// <remarks>
///     Тесты для <see cref="GetCurrentPortfolioStateUseCase" />
/// <list type="number">
///     <item>Должен вернуть текущее состояние портфеля, если оно есть</item>
///     <item>Должен вернуть NotFoundException, если портфель пуст</item>
///     <item>Должен вернуть ошибку, если хранилище бросает исключение</item>
/// </list>
/// </remarks>
public sealed class GetCurrentPortfolioStateUseCaseTests
{
    private readonly Mock<IDataStorageContext> _dataStorageMock = new();
    private readonly Mock<IRepository<PortfolioAsset>> _portfolioRepoMock = new();

    private readonly GetCurrentPortfolioStateUseCase _sut;

    /// <inheritdoc cref="GetCurrentPortfolioStateUseCaseTests" />
    public GetCurrentPortfolioStateUseCaseTests()
    {
        _dataStorageMock
            .Setup(x => x.PortfolioAssets)
            .Returns(_portfolioRepoMock.Object);

        _sut = new GetCurrentPortfolioStateUseCase(_dataStorageMock.Object);
    }

    /// <summary>
    ///     Тест 1: Должен вернуть текущее состояние портфеля, если оно есть
    /// </summary>
    [Fact(DisplayName = "Тест 1:  Должен вернуть текущее состояние портфеля, если оно есть")]
    public async Task Should_return_current_portfolio_when_present()
    {
        // Arrange
        var assets = new List<PortfolioAsset>
        {
            new() { Ticker = new() { Symbol = "AAPL" }, Quantity = 100 },
            new() { Ticker = new() { Symbol = "GOOG" }, Quantity = 50 },
        };

        _portfolioRepoMock
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync(assets);

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(assets);

        _portfolioRepoMock.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()), Times.Once);
    }

    /// <summary>
    ///     Тест 2: Должен вернуть NotFoundException, если портфель пуст
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен вернуть NotFoundException, если портфель пуст")]
    public async Task Should_return_not_found_when_portfolio_is_empty()
    {
        // Arrange
        _portfolioRepoMock
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ReturnsAsync(new List<PortfolioAsset>());

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(nameof(PortfolioAsset));

        _portfolioRepoMock.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()), Times.Once);
    }

    /// <summary>
    ///     Тест 3: Должен вернуть ошибку, если хранилище бросает исключение
    /// </summary>
    [Fact(DisplayName = "Тест 3: Должен вернуть ошибку, если хранилище бросает исключение")]
    public async Task Should_return_failure_when_repository_throws()
    {
        // Arrange
        _portfolioRepoMock
            .Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()))
            .ThrowsAsync(new Exception("storage is dead"));

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("storage is dead");

        _portfolioRepoMock.Verify(x => x.GetAllAsync(It.IsAny<Expression<Func<PortfolioAsset, bool>>>()), Times.Once);
    }
}
