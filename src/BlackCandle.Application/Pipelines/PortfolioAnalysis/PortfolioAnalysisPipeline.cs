using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines.AutoTradeExecution;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Пайплайн анализа портфеля
/// </summary>
public class PortfolioAnalysisPipeline : Pipeline<PortfolioAnalysisContext>
{
    /// <inheritdoc cref="PortfolioAnalysisPipeline" />
    public PortfolioAnalysisPipeline()
    { }

    /// <inheritdoc cref="PortfolioAnalysisPipeline" />
    public PortfolioAnalysisPipeline(
        IEnumerable<IPipelineStep<PortfolioAnalysisContext>> steps,
        ILoggerService logger)
        : base(steps, logger)
    { }

    /// <inheritdoc />
    public override string Name => "Анализ портфеля";
}
