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
        services.AddScoped<IPipelineStep<AutoTradeExecutionContext>, CheckAutoTradePermissionStep>();
        services.AddScoped<IPipelineStep<AutoTradeExecutionContext>, LoadSignalsStep>();
        services.AddScoped<IPipelineStep<AutoTradeExecutionContext>, ValidateTradeLimitsStep>();
        services.AddScoped<IPipelineStep<AutoTradeExecutionContext>, CalculateTradeVolumeStep>();
        services.AddScoped<IPipelineStep<AutoTradeExecutionContext>, PlaceOrdersStep>();
        services.AddScoped<IPipelineStep<AutoTradeExecutionContext>, UpdatePortfolioStep>();
        services.AddScoped<IPipelineStep<AutoTradeExecutionContext>, LogExecutedTradesStep>();
        services.AddScoped<AutoTradeExecutionPipeline>();

        return services;
    }
}
