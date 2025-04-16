using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;

using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Пайплайн анализа портфеля
/// </summary>
public class PortfolioAnalysisPipeline : Pipeline<PortfolioAnalysisContext>
{
    /// <inheritdoc cref="PortfolioAnalysisPipeline" />
    public PortfolioAnalysisPipeline(
        IServiceScope scope,
        ILoggerService logger)
        : base(scope, logger)
    { }

    /// <inheritdoc cref="PortfolioAnalysisPipeline" />
    protected PortfolioAnalysisPipeline()
    { }

    /// <inheritdoc />
    public override string Name => "Анализ портфеля";
}
