using BlackCandle.Application.Interfaces.Trading;
using BlackCandle.Application.Trading.SignalGeneration;

using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.Trading;

/// <summary>
///     Регистрация вспомогательных сервисов для торговли
/// </summary>
public static class TradingRegistration
{
    /// <summary>
    ///     Добавить генерацию сигналов
    /// </summary>
    public static IServiceCollection AddSignalGeneration(this IServiceCollection services)
    {
        services.AddSingleton<ISignalGenerationStrategy, DefaultSignalGenerationStrategy>();
        return services;
    }
}
