using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Infrastructure.InvestApi.SmartLab;
using BlackCandle.Infrastructure.InvestApi.Tinkoff;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Infrastructure.InvestApi;

/// <summary>
///     Хелпер для регистрации сервисов Invest API
/// </summary>
internal static class InvestApiRegistration
{
    /// <summary>
    ///     Добавить сервисы
    /// </summary>
    public static IServiceCollection AddInvestApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSmartLabScraper();
        services.AddTinkoffInvestApiServices(configuration);
        services.AddScoped<IInvestApiFacade, InvestApiFacade>();

        return services;
    }
}
