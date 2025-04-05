namespace BlackCandle.Domain.Enums;

/// <summary>
///     Статус сделки
/// </summary>
public enum TradeStatus
{
    /// <summary>
    ///     Пока не исполнена
    /// </summary>
    Pending,

    /// <summary>
    ///     Исполнена
    /// </summary>
    Success,

    /// <summary>
    ///     Ошибка
    /// </summary>
    Error,
}
