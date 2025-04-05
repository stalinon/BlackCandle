using BlackCandle.Application.Interfaces.InvestApi;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Tinkoff.InvestApi;

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
        var options = configuration.GetValue<TinkoffClientConfiguration>("Tinkoff")!;
        services.Configure<TinkoffClientConfiguration>(o =>
        {
            o.ApiKey = options.ApiKey;
            o.AccountId = options.ApiKey;
        });

        services.AddInvestApiClient((_, settings) => settings.AccessToken = options.ApiKey);

        services.AddSingleton<IMarketDataClient, TinkoffMarketDataClient>();
        services.AddSingleton<IPortfolioClient, TinkoffPortfolioClient>();
        services.AddSingleton<ITradingClient, TinkoffTradingClient>();

        return services;
    }
}
