using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Configuration;
using BlackCandle.Infrastructure.InvestApi;
using BlackCandle.Infrastructure.Logging;
using BlackCandle.Infrastructure.Persistence.Redis;
using BlackCandle.Infrastructure.Trading;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using StackExchange.Redis;

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
        services.AddSingleton<ILoggerService, ConsoleLogger>();
        services.AddInvestApiServices(configuration);
        services.AddTradingServices(configuration);
        services.RegisterRedis(configuration);

        return services;
    }

    private static IServiceCollection RegisterRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetValue<RedisOptions>("Redis") ?? new();
        services.Configure<RedisOptions>(o =>
        {
            o.Configuration = options.Configuration;
            o.Prefix = options.Prefix;
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var o = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(o.Configuration);
        });

        services.AddHostedService<RedisPingService>();

        return services;
    }
}
