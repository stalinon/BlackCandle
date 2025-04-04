using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Pipelines;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Пайплайн анализа портфеля
/// </summary>
public sealed class PortfolioAnalysisPipeline(IEnumerable<IPipelineStep<PortfolioAnalysisContext>> steps, ILoggerService logger) : Pipeline<PortfolioAnalysisContext>(steps, logger)
{
    /// <inheritdoc />
    protected override string Name => "Анализ портфеля";
}
