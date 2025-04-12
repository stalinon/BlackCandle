using BlackCandle.Application.Interfaces.InvestApi;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Хелпер для регистрации Tinkoff API сервисов
/// </summary>
internal static class TinkoffInvestApiRegistration
{
    /// <summary>
    ///     Добавить сервисы Tinkoff API
    /// </summary>
    public static IServiceCollection AddTinkoffInvestApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetValue<TinkoffClientConfiguration>("Tinkoff") ?? new();
        services.Configure<TinkoffClientConfiguration>(o =>
        {
            o.ApiKey = options.ApiKey;
            o.AccountId = options.ApiKey;
            o.UseSandbox = options.UseSandbox;
        });

        services.AddInvestApiClient((_, settings) =>
        {
            settings.AccessToken = options.ApiKey;
            settings.Sandbox = options.UseSandbox;
        });

        services.AddScoped<ITinkoffInvestApiClientWrapper, TinkoffInvestApiClientWrapper>();

        services.AddScoped<IMarketDataClient, TinkoffMarketDataClient>();
        services.AddScoped<IPortfolioClient, TinkoffPortfolioClient>();
        services.AddScoped<ITradingClient, TinkoffTradingClient>();
        services.AddScoped<IInstrumentClient, TinkoffInstrumentClient>();

        return services;
    }
}
