namespace BlackCandle.Domain.ValueObjects;

/// <summary>
///     Технический индикатор
/// </summary>
public class TechnicalIndicator
{
    /// <summary>
    ///     Название
    /// </summary>
    public string Name { get; set; } = default!;
    
    /// <summary>
    ///     Дата и время
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    ///     Значение
    /// </summary>
    public double? Value { get; set; }
    
    /// <summary>
    ///     Метаданные для MACD сигналов, трендов и т.д.
    /// </summary>
    public string? Meta { get; set; }
}
