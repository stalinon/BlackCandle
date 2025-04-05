namespace BlackCandle.Infrastructure.InvestApi.Tinkoff;

/// <summary>
///     Конфигурация клиента Tinkoff Invest API
/// </summary>
public class TinkoffClientConfiguration
{
    /// <summary>
    ///     Ключ API
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    ///     Идентификатор аккаунта
    /// </summary>
    public string AccountId { get; set; } = string.Empty;
}
