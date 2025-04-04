using BlackCandle.Infrastructure.InvestApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Infrastructure;

/// <summary>
///     Регистрация инфраструктуры
/// </summary>
public static class InfrastructureRegistration
{
    /// <summary>
    ///     Добавить сервисы инфраструктуры
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInvestApiServices(configuration);
        return services;
    }
}