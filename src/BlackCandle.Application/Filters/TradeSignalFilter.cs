using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Application.Filters;

/// <summary>
///     Фильтр <see cref="TradeSignal" />
/// </summary>
internal class TradeSignalFilter : IFilter<TradeSignal>
{
    /// <summary>
    ///     Дата сигналов
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    ///     Только купля/продажа
    /// </summary>
    public bool? OnlyBuySell { get; set; }

    /// <inheritdoc />
    public IQueryable<TradeSignal> Apply(IQueryable<TradeSignal> query)
    {
        if (Date.HasValue)
        {
            var start = Date.Value.Date;
            var end = start.AddDays(1);
            query = query.Where(s => s.Date >= start && s.Date < end);
        }

        if (OnlyBuySell == true)
        {
        }

        return query;
    }
}
