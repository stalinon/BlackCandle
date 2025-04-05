using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Tests.Models;

/// <summary>
///     Тестовая сущность
/// </summary>
internal class TestEntity : IEntity
{
    /// <inheritdoc />
    public required string Id { get; set; }

    /// <summary>
    ///     Название
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
