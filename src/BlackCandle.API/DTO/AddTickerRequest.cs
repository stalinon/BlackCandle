namespace BlackCandle.API.DTO;

/// <summary>
///     Запрос на добавление тикера
/// </summary>
public class AddTickerRequest
{
    /// <summary>
    ///     Символ
    /// </summary>
    public string Symbol { get; set; } = string.Empty;
    
    /// <summary>
    ///     Биржа
    /// </summary>
    public string Exchange { get; set; } = string.Empty;
    
    /// <summary>
    ///     Количество
    /// </summary>
    public decimal Quantity { get; set; }
    
    /// <summary>
    ///     Цена покупки
    /// </summary>
    public decimal PurchasePrice { get; set; }
    
    /// <summary>
    ///     Дата покупки
    /// </summary>
    public DateTime PurchaseDate { get; set; }
}