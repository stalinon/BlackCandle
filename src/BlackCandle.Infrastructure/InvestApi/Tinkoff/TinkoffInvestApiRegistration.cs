using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Helpers;

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
        services.AddInvestApiClient((sp, settings) =>
        {
            using var scope = sp.CreateScope();
            var botSettingsService = scope.ServiceProvider.GetRequiredService<IBotSettingsService>();
            var tinkoffConfig = botSettingsService.Get().ToTinkoffConfig();
            settings.AccessToken = tinkoffConfig.ApiKey;
            settings.Sandbox = tinkoffConfig.UseSandbox;
        });

        services.AddScoped<ITinkoffInvestApiClientWrapper, TinkoffInvestApiClientWrapper>();

        services.AddScoped<IMarketDataClient, TinkoffMarketDataClient>();
        services.AddScoped<IPortfolioClient, TinkoffPortfolioClient>();
        services.AddScoped<ITradingClient, TinkoffTradingClient>();
        services.AddScoped<IInstrumentClient, TinkoffInstrumentClient>();

        return services;
    }
}
