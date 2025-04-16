using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines;
using BlackCandle.Domain.Enums;
using BlackCandle.Infrastructure.Logging;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace BlackCandle.Tests.Application.Pipelines;

/// <remarks>
///     Тесты для <see cref="PipelineExecutionTracker{TContext}" />
/// <list type="number">
///     <item>Должен сохранить имя пайплайна, дату запуска и признак автозапуска</item>
///     <item>Должен отслеживать шаги: создать, обновить статус, завершить</item>
///     <item>Должен сохранять ошибку шага, если она передана</item>
///     <item>Должен завершить пайплайн, сохранить статус, дату и ошибку</item>
/// </list>
/// </remarks>
public sealed class PipelineExecutionTrackerTests
{
    /// <summary>
    ///     Тест
    /// </summary>
    public PipelineExecutionTrackerTests() { }

    /// <summary>
    ///     Тест 1: Должен сохранить имя пайплайна, дату запуска и признак автозапуска
    /// </summary>
    [Fact(DisplayName = "Тест 1: Должен сохранить имя пайплайна, дату запуска и признак автозапуска")]
    public void Should_capture_pipeline_name_and_start_time()
    {
        // Arrange
        var pipeline = new TestPipeline();
        var sut = new PipelineExecutionTracker<TestContext>();

        // Act
        sut.Attach(pipeline, wasScheduled: true);
        var record = sut.GetRecord();

        // Assert
        record.PipelineName.Should().Be("TestPipeline");
        record.WasScheduled.Should().BeTrue();
        record.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    /// <summary>
    ///        Тест 2: Должен отслеживать шаги: создать, обновить статус, завершить
    /// </summary>
    [Fact(DisplayName = "Тест 2: Должен отслеживать шаги: создать, обновить статус, завершить")]
    public async Task Should_track_step_status_changes()
    {
        // Arrange
        var pipeline = new TestPipeline("Step1");
        var sut = new PipelineExecutionTracker<TestContext>();
        sut.Attach(pipeline, false);

        // Act
        await pipeline.RunAsync(CancellationToken.None);
        var record = sut.GetRecord();
        var step = record.Steps.Should().ContainSingle(s => s.Name == "Step1").Subject;

        // Assert
        step.Status.Should().Be(PipelineStepStatus.Completed);
        step.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        step.FinishedAt.Should().NotBeNull();
        step.ErrorMessage.Should().BeNull();
    }

    /// <summary>
    ///        Тест 3: Должен сохранять ошибку шага, если она передана
    /// </summary>
    [Fact(DisplayName = "Тест 3: Должен сохранять ошибку шага, если она передана")]
    public async Task Should_record_step_error_if_exception_passed()
    {
        // Arrange
        var pipeline = new TestPipeline("FailingStep", throws: true);
        var sut = new PipelineExecutionTracker<TestContext>();
        sut.Attach(pipeline, false);

        // Act
        try
        {
            await pipeline.RunAsync(CancellationToken.None);
        }
        catch
        {
            // ignored
        }

        var step = sut.GetRecord().Steps.Should().ContainSingle(s => s.Name == "FailingStep").Subject;

        // Assert
        step.Status.Should().Be(PipelineStepStatus.Failed);
        step.ErrorMessage.Should().Be("fail");
        step.FinishedAt.Should().NotBeNull();
    }

    /// <summary>
    ///        Тест 4: Должен завершить пайплайн, сохранить статус, дату и ошибку
    /// </summary>
    [Fact(DisplayName = "Тест 4: Должен завершить пайплайн, сохранить статус, дату и ошибку")]
    public async Task Should_record_pipeline_completion_and_status()
    {
        // Arrange
        var pipeline = new TestPipeline("FinalStep", throws: true);
        var sut = new PipelineExecutionTracker<TestContext>();
        sut.Attach(pipeline, false);

        // Act
        try
        {
            await pipeline.RunAsync(CancellationToken.None);
        }
        catch
        {
            // ignored
        }

        var record = sut.GetRecord();

        // Assert
        record.Status.Should().Be(PipelineStatus.Failed);
        record.FinishedAt.Should().NotBeNull();
        record.ErrorMessage.Should().Be("fail");
    }

    private sealed class TestContext;

    private sealed class TestPipeline : Pipeline<TestContext>
    {
        public TestPipeline(string stepName = "Step1", bool throws = false)
            : base(Mock.Of<IServiceScope>(), new Logger())
        {
            AddStep(new FakeStep(stepName, throws));
        }

        public override string Name => "TestPipeline";
    }

    private sealed class FakeStep : IPipelineStep<TestContext>
    {
        private readonly bool _throws;

        /// <summary>
        ///     Шаг
        /// </summary>
        public FakeStep(string name, bool throws)
        {
            Name = name;
            _throws = throws;
            Logger = new ConsoleLogger();
        }

        public string Name { get; }

        public ILoggerService Logger { get; set; }

        public Action<TestContext, Exception, IPipelineStep<TestContext>> EarlyExitAction { get; set; } = null!;

        public PipelineStepStatus Status { get; set; }

        public Task ExecuteAsync(TestContext context, CancellationToken cancellationToken)
        {
            if (_throws)
            {
                throw new Exception("fail");
            }

            return Task.CompletedTask;
        }
    }
}
