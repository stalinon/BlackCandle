using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.InvestApi;
using BlackCandle.Domain.Entities;
using BlackCandle.Infrastructure.Persistence.InMemory;

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
        services.AddScoped<IFundamentalDataClient, SmartLabFundamentalClient>(
            sp => new SmartLabFundamentalClient(new InMemoryRepository<FundamentalData>(), sp.GetRequiredService<ILoggerService>()));
        return services;
    }
}
