namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Конфигурация клиента Tinkoff Invest API
/// </summary>
public class TinkoffClientConfiguration
{
    /// <summary>
    ///     Базовый URL песочницы
    /// </summary>
    public const string SandboxBaseUrl = "sandbox-invest-public-api.tinkoff.ru:443";

    /// <summary>
    ///     Ключ API
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    ///     Идентификатор аккаунта
    /// </summary>
    public string AccountId { get; set; } = string.Empty;

    /// <summary>
    ///     Использовать песочницу
    /// </summary>
    public bool UseSandbox { get; set; } = true;
}
