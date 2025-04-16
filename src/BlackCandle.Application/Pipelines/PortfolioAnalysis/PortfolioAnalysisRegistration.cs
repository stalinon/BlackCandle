using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines.PortfolioAnalysis.Steps;
using BlackCandle.Application.Trading;

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
        services.AddSignalGeneration();

        services.AddScoped<IPipelineStep<PortfolioAnalysisContext>, DiscoverNewTickersStep>();
        services.AddScoped<IPipelineStep<PortfolioAnalysisContext>, LoadPortfolioStep>();
        services.AddScoped<IPipelineStep<PortfolioAnalysisContext>, FetchMarketDataStep>();
        services.AddScoped<IPipelineStep<PortfolioAnalysisContext>, CalculateIndicatorsStep>();
        services.AddScoped<IPipelineStep<PortfolioAnalysisContext>, ScoreFundamentalsStep>();
        services.AddScoped<IPipelineStep<PortfolioAnalysisContext>, EvaluateTechnicalScoresStep>();
        services.AddScoped<IPipelineStep<PortfolioAnalysisContext>, GenerateSignalsStep>();
        services.AddScoped<IPipelineStep<PortfolioAnalysisContext>, LogStep>();

        return services;
    }
}
