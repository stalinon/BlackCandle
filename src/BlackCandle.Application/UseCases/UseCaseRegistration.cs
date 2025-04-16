using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Регистрация Use-case
/// </summary>
public static class UseCaseRegistration
{
    /// <summary>
    ///     Регистрация Use-case
    /// </summary>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<RunPortfolioAnalysisUseCase>();
        services.AddScoped<RunAutoTradeExecutionUseCase>();
        services.AddScoped<GetCurrentPortfolioStateUseCase>();
        services.AddScoped<GetLastAnalysisResultUseCase>();
        services.AddScoped<PreviewTradeExecutionUseCase>();

        return services;
    }
}
