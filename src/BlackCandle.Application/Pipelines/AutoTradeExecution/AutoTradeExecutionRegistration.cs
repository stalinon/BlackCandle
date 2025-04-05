using BlackCandle.Application.Interfaces.Pipelines;
using BlackCandle.Application.Pipelines.AutoTradeExecution.Steps;

using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.Pipelines.AutoTradeExecution;

/// <summary>
///     Регистрация пайплайна
/// </summary>
internal static class AutoTradeExecutionRegistration
{
    /// <summary>
    ///     Регистрация пайплайна
    /// </summary>
    public static IServiceCollection RegisterAutoTradeExecution(this IServiceCollection services)
    {
        services.AddTransient<IPipelineStep<AutoTradeExecutionContext>, CheckAutoTradePermissionStep>();
        services.AddTransient<IPipelineStep<AutoTradeExecutionContext>, LoadSignalsStep>();
        services.AddTransient<IPipelineStep<AutoTradeExecutionContext>, ValidateTradeLimitsStep>();
        services.AddTransient<IPipelineStep<AutoTradeExecutionContext>, CalculateTradeVolumeStep>();
        services.AddTransient<IPipelineStep<AutoTradeExecutionContext>, PlaceOrdersStep>();
        services.AddTransient<IPipelineStep<AutoTradeExecutionContext>, UpdatePortfolioStep>();
        services.AddTransient<IPipelineStep<AutoTradeExecutionContext>, LogExecutedTradesStep>();
        services.AddTransient<AutoTradeExecutionPipeline>();

        return services;
    }
}
