using BlackCandle.Domain.Entities;

namespace BlackCandle.Application.Interfaces.Trading;

/// <summary>
///     Валидатор сделок
/// </summary>
public interface ITradeLimitValidator
{
    /// <summary>
    ///     Провалидировать сигнал
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <listheader>Инкапсулирует такие проверки:</listheader>
    ///         <item>Доля позиции уже превышает допустимый лимит</item>
    ///         <item>Сумма сделки меньше минимального порога</item>
    ///         <item>Актив не ликвиден или запрещён</item>
    ///     </list>
    /// </remarks>
    Task<bool> Validate(TradeSignal signal, List<PortfolioAsset> portfolio);
}
