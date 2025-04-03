namespace BlackCandle.Domain.Interfaces;

/// <summary>
///     Базовый интерфейс сущности с идентификатором
/// </summary>
public interface IEntity
{
    /// <summary>
    ///     Уникальный идентификатор сущности
    /// </summary>
    string Id { get; }
}