using BlackCandle.Infrastructure.Persistence.InMemory;
using BlackCandle.Tests.Models;

namespace BlackCandle.Tests.Infrastructure;

/// <summary>
///     Тесты на <see href="InMemoryRepository{T}" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Добавление одной сущности и получение по ID</item>
///         <item>Добавление скопом и фильтрация по предикату</item>
///         <item>Удаление сущности по ID</item>
///         <item>Очистка хранилища</item>
///         <item>Попытка получить несуществующий ID</item>
///     </list>
/// </remarks>
public sealed class InMemoryRepositoryTests
{
    private InMemoryRepository<TestEntity> CreateRepository()
    {
        return new InMemoryRepository<TestEntity>();
    }

    /// <summary>
    ///     Тест 1: Добавление одной сущности и получение по ID
    /// </summary>
    [Fact(DisplayName = "Тест 1: Добавление одной сущности и получение по ID")]
    public async Task AddAsync_ShouldAddEntityAndRetrieveById()
    {
        // Arrange
        var repo = CreateRepository();
        var entity = new TestEntity { Id = "1", Name = "Test" };

        // Act
        await repo.AddAsync(entity);
        var result = await repo.GetByIdAsync("1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    /// <summary>
    ///     Тест 2: Добавление скопом и фильтрация по предикату
    /// </summary>
    [Fact(DisplayName = "Тест 2: Добавление скопом и фильтрация по предикату")]
    public async Task AddRangeAsync_ShouldAddMultipleEntities_AndFilterWithPredicate()
    {
        // Arrange
        var repo = CreateRepository();
        var items = new[]
        {
            new TestEntity { Id = "1", Name = "One" },
            new TestEntity { Id = "2", Name = "Two" },
            new TestEntity { Id = "3", Name = "Three" },
        };

        // Act
        await repo.AddRangeAsync(items);
        var filtered = await repo.GetAllAsync(x => x.Name.StartsWith("T"));

        // Assert
        Assert.Equal(2, filtered.Count);
    }

    /// <summary>
    ///     Тест 3: Удаление сущности по ID
    /// </summary>
    [Fact(DisplayName = "Тест 3: Удаление сущности по ID")]
    public async Task RemoveAsync_ShouldRemoveEntity()
    {
        // Arrange
        var repo = CreateRepository();
        var entity = new TestEntity { Id = "99", Name = "Temp" };
        await repo.AddAsync(entity);

        // Act
        await repo.RemoveAsync("99");
        var result = await repo.GetByIdAsync("99");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    ///     Тест 4: Очистка хранилища
    /// </summary>
    [Fact(DisplayName = "Тест 4: Очистка хранилища")]
    public async Task TruncateAsync_ShouldClearAllEntities()
    {
        // Arrange
        var repo = CreateRepository();
        await repo.AddAsync(new TestEntity { Id = "1", Name = "X" });

        // Act
        await repo.TruncateAsync();
        var all = await repo.GetAllAsync();

        // Assert
        Assert.Empty(all);
    }

    /// <summary>
    ///     Тест 5: Попытка получить несуществующий ID
    /// </summary>
    [Fact(DisplayName = "Тест 5: Попытка получить несуществующий ID")]
    public async Task GetByIdAsync_ShouldReturnNull_IfNotExists()
    {
        // Arrange
        var repo = CreateRepository();

        // Act
        var result = await repo.GetByIdAsync("nonexistent");

        // Assert
        Assert.Null(result);
    }
}
