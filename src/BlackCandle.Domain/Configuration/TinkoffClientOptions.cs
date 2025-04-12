using BlackCandle.Domain.Attributes;

namespace BlackCandle.Domain.Configuration;

/// <summary>
///     Конфигурация клиента Tinkoff Invest API
/// </summary>
public class TinkoffClientOptions
{
    /// <summary>
    ///     Ключ API
    /// </summary>
    [Secret]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    ///     Идентификатор аккаунта
    /// </summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    ///     Использовать песочницу
    /// </summary>
    public bool UseSandbox { get; set; } = true;

    /// <summary>
    ///     Клонировать сущность
    /// </summary>
    public TinkoffClientOptions Copy() => (TinkoffClientOptions)MemberwiseClone();
}
