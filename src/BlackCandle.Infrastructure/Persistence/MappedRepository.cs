using System.Linq.Expressions;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Interfaces;

namespace BlackCandle.Infrastructure.Persistence;

/// <summary>
///     Репозиторий с маппингом для внешних хранилищ
/// </summary>
public abstract class MappedRepository<TDomain, TStorage> : IRepository<TDomain>
    where TDomain : IEntity
    where TStorage : IStorageEntity<TDomain>, new()
{
    /// <inheritdoc />
    public async Task<List<TDomain>> GetAllAsync(IFilter<TDomain>? filter)
    {
        var storageModels = await GetStorageAsync(filter);
        return storageModels.Select(MapToDomain).ToList();
    }

    /// <inheritdoc />
    public async Task<List<TDomain>> GetAllAsync(Expression<Func<TDomain, bool>>? predicate = null)
    {
        if (predicate is null)
        {
            return await GetAllAsync(filter: null);
        }

        throw new NotSupportedException("Expression filters not supported in this repositories");
    }

    /// <inheritdoc />
    public async Task<TDomain?> GetByIdAsync(string id)
    {
        var model = await GetStorageByIdAsync(id);
        return model is null ? default : MapToDomain(model);
    }

    /// <inheritdoc />
    public Task AddAsync(TDomain entity) => AddStorageAsync(MapToStorage(entity));

    /// <inheritdoc />
    public Task RemoveAsync(string id) => RemoveStorageAsync(id);

    /// <inheritdoc />
    public Task AddRangeAsync(IEnumerable<TDomain> entities) =>
        AddRangeStorageAsync(entities.Select(MapToStorage));

    /// <inheritdoc />
    public Task TruncateAsync() => TruncateStorageAsync();

    /// <inheritdoc cref="GetAllAsync(BlackCandle.Domain.Interfaces.IFilter{TDomain}?)" />
    protected abstract Task<List<TStorage>> GetStorageAsync(IFilter<TDomain>? filter);

    /// <inheritdoc cref="GetByIdAsync" />
    protected abstract Task<TStorage?> GetStorageByIdAsync(string id);

    /// <inheritdoc cref="AddAsync" />
    protected abstract Task AddStorageAsync(TStorage model);

    /// <inheritdoc cref="RemoveAsync" />
    protected abstract Task RemoveStorageAsync(string id);

    /// <inheritdoc cref="AddRangeAsync" />
    protected abstract Task AddRangeStorageAsync(IEnumerable<TStorage> models);

    /// <inheritdoc cref="TruncateAsync" />
    protected abstract Task TruncateStorageAsync();

    /// <summary>
    ///     Маппить
    /// </summary>
    protected abstract TDomain MapToDomain(TStorage storage);

    /// <summary>
    ///     Маппить
    /// </summary>
    protected abstract TStorage MapToStorage(TDomain domain);
}
