namespace BlackCandle.Domain.Interfaces;

/// <summary>
///     Фильтр данных
/// </summary>
public interface IFilter<T>
{
    /// <summary>
    ///     Применить
    /// </summary>
    IQueryable<T> Apply(IQueryable<T> query);
}
