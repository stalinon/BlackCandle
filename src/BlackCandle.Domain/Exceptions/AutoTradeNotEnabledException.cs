namespace BlackCandle.Domain.Exceptions;

/// <summary>
///     Исключение при попытке запуска автотрейдинга, когда он отключен
/// </summary>
public sealed class AutoTradeNotEnabledException : BlackCandleException
{
    /// <inheritdoc cref="AutoTradeNotEnabledException" />
    public AutoTradeNotEnabledException()
        : base("Режим автоматической торговли отключен настройками бота.")
    {
    }
}
