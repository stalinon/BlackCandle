using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Services;
using BlackCandle.Infrastructure.Persistence;

namespace BlackCandle.API.Configuration;

/// <summary>
///     Конфигурация DI
/// </summary>
public static class DependencyInjectionConfig
{
    /// <summary>
    ///     Добавить сервисы проекта
    /// </summary>
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddSingleton<IDataStorageContext, InMemoryDataStorageContext>();
        services.AddScoped<IPortfolioService, PortfolioService>();

        return services;
    }
}