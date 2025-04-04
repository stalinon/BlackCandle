using BlackCandle.Application.Pipelines.PortfolioAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.Pipelines;

/// <summary>
///     Регистрация пайплайнов
/// </summary>
public static class PipelineRegistration
{
    /// <summary>
    ///     Регистрация пайплайнов
    /// </summary>
    public static IServiceCollection RegisterPipelines(this IServiceCollection services)
    {
        services.RegisterPortfolioAnalysis();
        return services;
    }
}