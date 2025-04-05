using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;

namespace BlackCandle.Domain.ValueObjects;

/// <summary>
///     Превью заявки
/// </summary>
/// <param name="Ticker">Тикер</param>
/// <param name="Side">Действие сделки</param>
/// <param name="Quantity">Количество лотов</param>
/// <param name="Price">Цена лота</param>
public record OrderPreview(Ticker Ticker, TradeAction Side, decimal Quantity, decimal Price);
