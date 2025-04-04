using BlackCandle.Application.Interfaces.InvestApi;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Infrastructure.InvestApi.SmartLab;

/// <summary>
///     Регистрация скрапера Smart Lab
/// </summary>
internal static class SmartLabScraperRegistration
{
    /// <summary>
    ///     Добавить скрапер Smart-Lab
    /// </summary>
    public static IServiceCollection AddSmartLabScraper(this IServiceCollection services)
    {
        services.AddSingleton<IFundamentalDataClient, SmartLabFundamentalClient>();
        return services;
    }
}