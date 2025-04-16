using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines.AutoTradeExecution;
using BlackCandle.Application.UseCases;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.ValueObjects;

using FluentAssertions;

using Moq;

namespace BlackCandle.Tests.Application.UseCases;

/// <remarks>
///     Тесты для <see cref="PreviewTradeExecutionUseCase" />
/// <list type="number">
///     <item>Должен вернуть список ордеров предпросмотра, если пайплайн завершился успешно</item>
///     <item>Должен вернуть ошибку, если пайплайн завершился неуспешно</item>
/// </list>
/// </remarks>
public sealed class PreviewTradeExecutionUseCaseTests
{
    private readonly Mock<AutoTradeExecutionPipeline> _pipeline = new();

    private readonly PreviewTradeExecutionUseCase _sut;

    /// <inheritdoc cref="PreviewTradeExecutionUseCaseTests" />
    public PreviewTradeExecutionUseCaseTests()
    {
        _pipeline.SetupGet(p => p.Context)
            .Returns(new AutoTradeExecutionContext());

        var factory = new Mock<IPipelineFactory>();
        factory.Setup(f => f.Create<AutoTradeExecutionPipeline, AutoTradeExecutionContext>()).Returns(_pipeline.Object);
        _sut = new PreviewTradeExecutionUseCase(factory.Object);
    }

    /// <summary>
    ///     Тест 1: Должен вернуть список ордеров предпросмотра, если пайплайн завершился успешно
    /// </summary>
    [Fact(DisplayName = "Тест 1: Должен вернуть список ордеров предпросмотра, если пайплайн завершился успешно")]
    public async Task Should_return_preview_orders_when_pipeline_completed()
    {
        // Arrange
        var expected = new List<OrderPreview>
        {
            new(new() { Symbol = "AAPL" }, TradeAction.Buy, 10, 187.45m),
        };

        _pipeline.SetupGet(p => p.Context).Returns(new AutoTradeExecutionContext
        {
            PreviewOrders = expected,
        });
        _pipeline.SetupGet(p => p.Status).Returns(PipelineStatus.Completed);
        _pipeline.Setup(p => p.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expected);

        _pipeline.Object.Context.PreviewMode.Should().BeTrue();
        _pipeline.Verify(p => p.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Тест 2: Должен вернуть ошибку, если пайплайн завершился неуспешно
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен вернуть ошибку, если пайплайн завершился неуспешно")]
    public async Task Should_return_failure_when_pipeline_fails()
    {
        // Arrange
        _pipeline.Setup(p => p.RunAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _pipeline.SetupGet(p => p.Status)
            .Returns(PipelineStatus.Failed);

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Не удалось");

        _pipeline.Object.Context.PreviewMode.Should().BeTrue();
        _pipeline.Verify(p => p.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
