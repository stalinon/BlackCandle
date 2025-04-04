using System.Collections.Concurrent;
using System.Linq.Expressions;
using BlackCandle.Application.Interfaces;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence.InMemory;

/// <summary>
/// Хранилище сущностей в оперативной памяти
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
internal sealed class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private readonly ConcurrentDictionary<string, T> _storage = new();

    /// <inheritdoc />
    public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var result = predicate is null ? _storage.Values : _storage.Values.Where(predicate.Compile());
        return Task.FromResult(result.ToList());
    }

    /// <inheritdoc />
    public Task<T?> GetByIdAsync(string id)
        => Task.FromResult(_storage.GetValueOrDefault(id));

    /// <inheritdoc />
    public Task AddAsync(T entity)
    {
        _storage[entity.Id] = entity;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(string id)
    {
        _storage.Remove(id, out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task AddRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            _storage[entity.Id] = entity;
        }
        
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task TruncateAsync()
    {
        _storage.Clear();
        return Task.CompletedTask;
    }
}