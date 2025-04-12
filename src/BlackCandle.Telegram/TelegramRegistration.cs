using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Configuration;

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
        services.AddScoped<ITelegramService, TelegramService>();
        return services;
    }
}
