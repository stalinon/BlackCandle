using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Domain.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Infrastructure.Trading;

/// <summary>
///     Регистрация сервисов трейдинга
/// </summary>
internal static class TradingRegistration
{
    /// <summary>
    ///     Регистрация сервисов трейдинга
    /// </summary>
    public static IServiceCollection AddTradingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITradeLimitValidator, TradeLimitValidator>();
        services.AddScoped<ITradeExecutionService, TradeExecutionService>();

        return services;
    }
}
