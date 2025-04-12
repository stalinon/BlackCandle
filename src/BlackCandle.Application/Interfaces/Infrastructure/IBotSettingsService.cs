using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces.Infrastructure;

/// <summary>
///     Сервис настройки бота
/// </summary>
public interface IBotSettingsService
{
    /// <summary>
    ///     Получить настройки бота
    /// </summary>
    Task<BotSettings> GetAsync();

    /// <summary>
    ///     Сохранить настройки
    /// </summary>
    Task SaveAsync(BotSettings settings);
}
