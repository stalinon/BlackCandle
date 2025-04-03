namespace BlackCandle.Domain.ValueObjects;

/// <summary>
///     Историческая точка по OHLCV-данным.
/// </summary>
public class PriceHistoryPoint
{
    /// <summary>
    ///     Дата и время
    /// </summary>
    public DateTime Date { get; init; }
    
    /// <summary>
    ///     Цена открытия
    /// </summary>
    public decimal Open { get; init; }
    
    /// <summary>
    ///     Максимальная цена
    /// </summary>
    public decimal High { get; init; }
    
    /// <summary>
    ///     Минимальная цена
    /// </summary>
    public decimal Low { get; init; }
    
    /// <summary>
    ///     Цена закрытия
    /// </summary>
    public decimal Close { get; init; }
    
    /// <summary>
    ///     Объем
    /// </summary>
    public long Volume { get; init; }
}