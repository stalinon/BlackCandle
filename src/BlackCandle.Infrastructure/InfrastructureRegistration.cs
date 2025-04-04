using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Infrastructure.InvestApi;
using BlackCandle.Infrastructure.Logging;
using BlackCandle.Infrastructure.Trading;
using Microsoft.Extensions.DependencyInjection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

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
        services.AddScoped<ILoggerService, ConsoleLogger>();
        
        services.AddInvestApiServices(configuration);

        services.AddTradingServices(configuration);
        
        return services;
    }
}