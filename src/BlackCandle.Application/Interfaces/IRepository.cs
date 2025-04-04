using System.Linq.Expressions;

namespace BlackCandle.Application.Interfaces;

/// <summary>
///     Универсальный репозиторий для работы с сущностями
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public interface IRepository<T>
{
    /// <summary>
    ///     Получить все сущности
    /// </summary>
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    ///     Получить сущность по идентификатору
    /// </summary>
    Task<T?> GetByIdAsync(string id);

    /// <summary>
    ///     Добавить сущность
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    ///     Удалить сущность по идентификатору
    /// </summary>
    Task RemoveAsync(string id);
    
    /// <summary>
    ///     Добавить скопом
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    ///     Очистить хранилище
    /// </summary>
    Task TruncateAsync();
}