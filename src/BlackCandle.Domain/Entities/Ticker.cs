namespace BlackCandle.Domain.Entities;

/// <summary>
///     Тикер
/// </summary>
public class Ticker
{
    /// <summary>
    ///     Символ тикера
    /// </summary>
    public string Symbol { get; set; } = string.Empty;
    
    /// <summary>
    ///     Биржа
    /// </summary>
    public string Exchange { get; set; } = string.Empty;
}