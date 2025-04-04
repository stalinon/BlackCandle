using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.Pipelines.PortfolioAnalysis;

/// <summary>
///     Регистрация пайплайна
/// </summary>
internal static class PortfolioAnalysisRegistration
{
    /// <summary>
    ///     Зарегистрировать пайплайн
    /// </summary>
    public static IServiceCollection RegisterPortfolioAnalysis(this IServiceCollection services)
    {
        services.AddTransient<IPipelineStep<PortfolioAnalysisContext>, LoadPortfolioStep>();
        services.AddTransient<IPipelineStep<PortfolioAnalysisContext>, FetchMarketDataStep>();
        services.AddTransient<IPipelineStep<PortfolioAnalysisContext>, CalculateIndicatorsStep>();
        services.AddTransient<IPipelineStep<PortfolioAnalysisContext>, ScoreFundamentalsStep>();
        services.AddTransient<IPipelineStep<PortfolioAnalysisContext>, GenerateSignalsStep>();
        services.AddTransient<IPipelineStep<PortfolioAnalysisContext>, LogStep>();
        services.AddTransient<PortfolioAnalysisPipeline>();

        return services;
    }
}