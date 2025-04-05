using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.UseCases;
using BlackCandle.Domain.Enums;

using FluentAssertions;

using Moq;

namespace BlackCandle.Tests.Application.UseCases;

/// <remarks>
///     Тесты для <see cref="RunAutoTradeExecutionUseCase" />
/// <list type="number">
///   <item>Должен вернуть успешный результат, если пайплайн завершился успешно</item>
///   <item>Должен вернуть ошибку, если пайплайн завершился с ошибкой</item>
/// </list>
/// </remarks>
public sealed class RunAutoTradeExecutionUseCaseTests
{
    private readonly Mock<AutoTradeExecutionPipeline> _pipeline = new();

    private readonly RunAutoTradeExecutionUseCase _sut;

    /// <inheritdoc cref="RunAutoTradeExecutionUseCaseTests" />
    public RunAutoTradeExecutionUseCaseTests()
    {
        _sut = new RunAutoTradeExecutionUseCase(_pipeline.Object);
    }

    /// <summary>
    ///     Тест 1: Должен вернуть успешный результат, если пайплайн завершился успешно
    /// </summary>
    [Fact(DisplayName = "Тест 1: Должен вернуть успешный результат, если пайплайн завершился успешно")]
    public async Task Should_return_success_when_pipeline_completed()
    {
        // Arrange
        _pipeline
            .Setup(p => p.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _pipeline
            .SetupGet(p => p.Status)
            .Returns(PipelineStatus.Completed);

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Contain("успешно");

        _pipeline.Verify(p => p.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Тест 2: Должен вернуть ошибку, если пайплайн завершился с ошибкой
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен вернуть ошибку, если пайплайн завершился с ошибкой")]
    public async Task Should_return_failure_when_pipeline_failed()
    {
        // Arrange
        _pipeline
            .Setup(p => p.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _pipeline
            .SetupGet(p => p.Status)
            .Returns(PipelineStatus.Failed);

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ошибка");

        _pipeline.Verify(p => p.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
