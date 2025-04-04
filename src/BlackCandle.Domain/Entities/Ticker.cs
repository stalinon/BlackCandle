namespace BlackCandle.Domain.Entities;

/// <summary>
///     Тикер (инструмент)
/// </summary>
public class Ticker
{
    /// <summary>
    ///     Символ
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    ///     Валюта
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    ///     Отрасль
    /// </summary>
    public string Sector { get; set; } = string.Empty;

    /// <summary>
    ///     Уникальный код тикера
    /// </summary>
    public string Figi { get; set; } = string.Empty;
}