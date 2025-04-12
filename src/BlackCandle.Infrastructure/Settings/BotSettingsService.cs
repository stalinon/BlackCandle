using System.Reflection;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Attributes;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Exceptions;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Settings;

/// <inheritdoc cref="IBotSettingsService" />
public sealed class BotSettingsService(IDataStorageContext context, ISecretsProtector protector) : IBotSettingsService
{
    private const string DefaultId = BotSettings.DefaultId;

    /// <summary>
    ///     Получить настройки
    /// </summary>
    public async Task<BotSettings> GetAsync()
    {
        var settings = await context.BotSettings.GetByIdAsync(DefaultId);
        if (settings is null)
        {
            throw new BotNotConfiguredException();
        }

        return Decrypt(settings);
    }

    /// <summary>
    ///     Сохранить настройки
    /// </summary>
    public async Task SaveAsync(BotSettings settings)
    {
        var encrypted = Encrypt(settings);
        await context.BotSettings.AddAsync(encrypted);
    }

    private static IEnumerable<(object Owner, PropertyInfo Prop)> GetAllSecretProps(object? obj)
    {
        if (obj is null)
        {
            yield break;
        }

        var type = obj.GetType();
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0);

        foreach (var prop in props)
        {
            if (Attribute.IsDefined(prop, typeof(SecretAttribute)))
            {
                yield return (obj, prop);
            }

            var nestedValue = prop.GetValue(obj);
            if (nestedValue is null || prop.PropertyType == typeof(string) || prop.PropertyType.IsPrimitive)
            {
                continue;
            }

            foreach (var nested in GetAllSecretProps(nestedValue))
            {
                yield return nested;
            }
        }
    }

    private BotSettings Encrypt(BotSettings settings)
    {
        var clone = settings.Copy();

        foreach (var (owner, prop) in GetAllSecretProps(clone))
        {
            var value = prop.GetValue(owner) as string;
            if (!string.IsNullOrWhiteSpace(value))
            {
                prop.SetValue(owner, protector.Encrypt(value));
            }
        }

        return clone;
    }

    private BotSettings Decrypt(BotSettings settings)
    {
        var clone = settings.Copy();

        foreach (var (owner, prop) in GetAllSecretProps(clone))
        {
            var value = prop.GetValue(owner) as string;
            if (!string.IsNullOrWhiteSpace(value))
            {
                prop.SetValue(owner, protector.Decrypt(value));
            }
        }

        return clone;
    }
}
