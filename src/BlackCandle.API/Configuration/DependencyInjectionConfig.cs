using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Pipelines;
using BlackCandle.Infrastructure;
using BlackCandle.Infrastructure.Persistence.InMemory;

namespace BlackCandle.API.Configuration;

/// <summary>
///     Конфигурация DI
/// </summary>
public static class DependencyInjectionConfig
{
    /// <summary>
    ///     Добавить сервисы проекта
    /// </summary>
    public static IServiceCollection AddProjectServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDataStorageContext, InMemoryDataStorageContext>();
        services.RegisterPipelines();
        services.AddInfrastructure(configuration);

        return services;
    }
}