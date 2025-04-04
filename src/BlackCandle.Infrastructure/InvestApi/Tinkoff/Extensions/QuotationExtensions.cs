using Tinkoff.InvestApi.V1;

namespace BlackCandle.Infrastructure.InvestApi.Tinkoff.Extensions;

/// <summary>
///     Расширения для <see cref="Quotation"/>
/// </summary>
internal static class QuotationExtensions
{
    /// <summary>
    ///     Привести к адекватному виду
    /// </summary>
    public static decimal ToDecimal(this Quotation q)
    {
        return q.Units + q.Nano / 1_000_000_000M;
    }
    
    /// <summary>
    ///     Привести к адекватному виду
    /// </summary>
    public static decimal ToDecimal(this MoneyValue q)
    {
        return q.Units + q.Nano / 1_000_000_000M;
    }
}
