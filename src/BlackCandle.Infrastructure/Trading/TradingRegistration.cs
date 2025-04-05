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
        var tradeLimits = configuration.GetValue<TradeLimitOptions>("TradeLimits") ?? new TradeLimitOptions();
        services.Configure<TradeLimitOptions>(o =>
        {
            o.MinTradeAmountRub = tradeLimits.MinTradeAmountRub;
            o.MaxPositionSharePercent = tradeLimits.MaxPositionSharePercent;
        });
        services.AddScoped<ITradeLimitValidator, TradeLimitValidator>();

        var tradeExecution = configuration.GetValue<TradeExecutionOptions>("TradeExecution") ??
                             new TradeExecutionOptions();
        services.Configure<TradeExecutionOptions>(o =>
        {
            o.MaxTradeAmountRub = tradeExecution.MaxTradeAmountRub;
            o.MaxLotsPerTrade = tradeExecution.MaxLotsPerTrade;
        });
        services.AddScoped<ITradeExecutionService, TradeExecutionService>();

        return services;
    }
}
