using BlackCandle.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackCandle.Application.Interfaces;

namespace BlackCandle.Infrastructure.Persistence;

/// <summary>
/// Хранилище сущностей в оперативной памяти
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private readonly Dictionary<string, T> _storage = new();

    /// <inheritdoc />
    public Task<List<T>> GetAllAsync() => Task.FromResult(_storage.Values.ToList());

    /// <inheritdoc />
    public Task<T?> GetByIdAsync(string id)
        => Task.FromResult(_storage.TryGetValue(id, out var entity) ? entity : default);

    /// <inheritdoc />
    public Task AddAsync(T entity)
    {
        _storage[entity.Id] = entity;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string id)
    {
        _storage.Remove(id);
        return Task.CompletedTask;
    }
}