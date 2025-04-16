using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using BlackCandle.Application.UseCases;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

using FluentAssertions;

using Moq;

namespace BlackCandle.Tests.Application.UseCases;

/// <summary>
///     Тесты для <see cref="RunPortfolioAnalysisUseCase" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Должен вернуть успешный результат, если пайплайн завершился успешно</item>
///         <item>Должен вернуть ошибку, если пайплайн завершился с ошибкой</item>
///     </list>
/// </remarks>
public sealed class RunPortfolioAnalysisUseCaseTests
{
    private readonly Mock<PortfolioAnalysisPipeline> _pipeline = new();
    private readonly Mock<IRepository<PipelineExecutionRecord>> _repository = new();
    private readonly RunPortfolioAnalysisUseCase _sut;

    /// <inheritdoc cref="RunPortfolioAnalysisUseCaseTests"/>
    public RunPortfolioAnalysisUseCaseTests()
    {
        var contextMock = new Mock<IDataStorageContext>();
        contextMock.SetupGet(x => x.PipelineRuns).Returns(_repository.Object);
        _repository.Setup(x => x.AddAsync(It.IsAny<PipelineExecutionRecord>())).Returns(Task.CompletedTask);

        var factory = new Mock<IPipelineFactory>();
        factory.Setup(f => f.Create<PortfolioAnalysisPipeline, PortfolioAnalysisContext>()).Returns(_pipeline.Object);
        _sut = new RunPortfolioAnalysisUseCase(factory.Object, contextMock.Object);
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
        result.Data.Should().Contain("проанализирован");

        _pipeline.Verify(p => p.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.AddAsync(It.IsAny<PipelineExecutionRecord>()), Times.Once);
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
        result.Error.Should().Contain("Не удалось");

        _pipeline.Verify(p => p.RunAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.AddAsync(It.IsAny<PipelineExecutionRecord>()), Times.Once);
    }
}
