using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines;
using BlackCandle.Domain.Enums;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines;

/// <summary>
///     Тесты на <see cref="Pipeline{TContext}" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Пайплайн проходит все шаги при успехе</item>
///         <item>Пайплайн прерывается при исключении</item>
///         <item>EarlyExit прерывает пайплайн</item>
///         <item>События статусов шага и пайплайна вызываются корректно</item>
///     </list>
/// </remarks>
public sealed class PipelineTests
{
    /// <summary>
    ///     Тест 1: Пайплайн проходит все шаги при успехе
    /// </summary>
    [Fact(DisplayName = "Тест 1: Пайплайн проходит все шаги при успехе")]
    public async Task RunAsync_ShouldCompletePipeline_WhenAllStepsSucceed()
    {
        // Arrange
        var logger = new Mock<ILoggerService>();
        var steps = new[]
        {
            CreateStep("Step1"),
            CreateStep("Step2"),
        };

        var pipeline = new TestPipeline(steps.Select(x => x), logger.Object);

        // Act
        await pipeline.RunAsync();

        // Assert
        Assert.Equal(PipelineStatus.Completed, pipeline.Status);
        foreach (var step in steps)
        {
            Assert.Equal(PipelineStepStatus.Completed, step.Status);
        }
    }

    /// <summary>
    ///     Тест 2: Пайплайн прерывается при исключении
    /// </summary>
    [Fact(DisplayName = "Тест 2: Пайплайн прерывается при исключении")]
    public async Task RunAsync_ShouldFailPipeline_WhenStepThrows()
    {
        // Arrange
        var logger = new Mock<ILoggerService>();
        var step1 = CreateStep("Step1");
        var step2 = CreateStep("Step2", true);
        var pipeline = new TestPipeline(new[] { step1, step2 }, logger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => pipeline.RunAsync());
        Assert.Equal(PipelineStatus.Failed, pipeline.Status);
        Assert.Equal(PipelineStepStatus.Completed, step1.Status);
        Assert.Equal(PipelineStepStatus.Failed, step2.Status);
    }

    /// <summary>
    ///     Тест 3: EarlyExit прерывает пайплайн
    /// </summary>
    [Fact(DisplayName = "Тест 3: EarlyExit прерывает пайплайн")]
    public async Task RunAsync_ShouldNotContinue_WhenEarlyExitIsTriggered()
    {
        // Arrange
        var logger = new Mock<ILoggerService>();

        var step1 = CreateStep("Step1");
        var step2 = CreateStep("Step2");

        step1.Action = () => step1.EarlyExitAction(new TestContext(), new Exception("Early exit"), step1);

        var pipeline = new TestPipeline([step1, step2], logger.Object);

        // Act
        await pipeline.RunAsync();

        // Assert
        Assert.Equal(PipelineStatus.NotStarted, pipeline.Status);
        Assert.Equal(PipelineStepStatus.Completed, step1.Status); // всё же completed
        Assert.Equal(PipelineStepStatus.NotStarted, step2.Status); // не вызван
    }

    /// <summary>
    ///     Тест 4: События статусов шага и пайплайна вызываются корректно
    /// </summary>
    [Fact(DisplayName = "Тест 4: События статусов шага и пайплайна вызываются корректно")]
    public async Task RunAsync_ShouldRaiseStatusEvents()
    {
        // Arrange
        var logger = new Mock<ILoggerService>();
        var step = CreateStep("Step1");

        var pipeline = new TestPipeline(new[] { step }, logger.Object);

        var pipelineEvents = new List<PipelineStatus>();
        var stepEvents = new List<PipelineStepStatus>();

        pipeline.OnStatusChanged += (status, _, _) => pipelineEvents.Add(status);
        pipeline.OnStepStatusChanged += (status, _, _, _) => stepEvents.Add(status);

        // Act
        await pipeline.RunAsync();

        // Assert
        Assert.Equal(new[] { PipelineStatus.Running, PipelineStatus.Completed }, pipelineEvents);
        Assert.Contains(PipelineStepStatus.Completed, stepEvents);
    }

    private static TestStepPipeline CreateStep(string name, bool throws = false)
    {
        return new TestStepPipeline
        {
            StepName = name,
            Throws = throws,
        };
    }

    private class TestContext;

    private sealed class TestPipeline(IEnumerable<IPipelineStep<TestContext>> steps, ILoggerService logger)
        : Pipeline<TestContext>(steps, logger)
    {
        protected override string Name => "TestPipeline";
    }

    private sealed class TestStepPipeline : IPipelineStep<TestContext>
    {
        /// <inheritdoc />
        public ILoggerService Logger { get; set; } = null!;

        /// <inheritdoc />
        public Action<TestContext, Exception, IPipelineStep<TestContext>> EarlyExitAction { get; set; } = null!;

        /// <inheritdoc />
        public PipelineStepStatus Status { get; set; }

        /// <inheritdoc />
        public string StepName { get; set; } = null!;

        /// <summary>
        ///     Выбрасывает исключение
        /// </summary>
        public bool Throws { get; set; }

        /// <summary>
        ///     Выполнить действие
        /// </summary>
        public Action? Action { get; set; }

        /// <inheritdoc />
        public async Task ExecuteAsync(TestContext context, CancellationToken cancellationToken = default)
        {
            if (Action != null)
            {
                Action();
                return;
            }

            if (Throws)
            {
                throw new InvalidOperationException("Step error");
            }

            await Task.CompletedTask;
        }
    }
}
