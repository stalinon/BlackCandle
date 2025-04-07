namespace BlackCandle.Domain.Interfaces;

/// <summary>
///     Сущность-адаптер
/// </summary>
public interface IStorageEntity<TEntity>
    where TEntity : IEntity
{
    /// <summary>
    ///     Преобразовать к обычной сущности
    /// </summary>
    TEntity ToEntity();

    /// <summary>
    ///     Преобразовать к сущности-адаптеру
    /// </summary>
    IStorageEntity<TEntity> ToStorageEntity(TEntity entity);
}
