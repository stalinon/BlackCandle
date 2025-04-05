using BlackCandle.Application.Interfaces.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Telegram;

/// <summary>
///     Регистрация сервисов TG
/// </summary>
public static class TelegramRegistration
{
    /// <summary>
    ///     Регистрация сервисов TG
    /// </summary>
    public static IServiceCollection AddTelegram(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<TelegramOptions>(config.GetSection("Telegram"));
        services.AddScoped<ITelegramService, TelegramService>();
        return services;
    }
}
