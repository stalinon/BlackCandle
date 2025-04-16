using System.Text.Json;

using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Interfaces;

using StackExchange.Redis;

namespace BlackCandle.Infrastructure.Persistence.Redis;

/// <summary>
///     Реализация репозитория для Redis
/// </summary>
internal sealed class RedisRepository<TDomain, TStorage> : MappedRepository<TDomain, TStorage>
    where TDomain : IEntity
    where TStorage : IStorageEntity<TDomain>, new()
{
    private readonly IDatabase _db;
    private readonly string _prefix;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc cref="RedisRepository{TDomain,TStorage}" />
    public RedisRepository(IConnectionMultiplexer redis, RedisOptions options)
    {
        _db = redis.GetDatabase();
        _prefix = options.Prefix + typeof(TDomain).Name.ToLower() + ":";
    }

    /// <inheritdoc />
    protected override async Task<List<TStorage>> GetStorageAsync(IFilter<TDomain>? filter)
    {
        var keys = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0])
            .Keys(pattern: _prefix + "*")
            .ToArray();

        var values = await _db.StringGetAsync(keys);
        var result = values
            .Where(v => v.HasValue)
            .Select(v => JsonSerializer.Deserialize<TStorage>(v!, _jsonOptions)!)
            .ToList();

        if (filter is not null)
        {
            var domainList = result.Select(MapToDomain).AsQueryable();
            domainList = filter.Apply(domainList);
            return domainList.AsEnumerable().Select(MapToStorage).ToList();
        }

        return result;
    }

    /// <inheritdoc />
    protected override TStorage? GetStorageById(string id)
    {
        var raw = _db.StringGet(Key(id));
        return raw.HasValue ? JsonSerializer.Deserialize<TStorage>(raw!, _jsonOptions) : default;
    }

    /// <inheritdoc />
    protected override async Task<TStorage?> GetStorageByIdAsync(string id)
    {
        var raw = await _db.StringGetAsync(Key(id));
        return raw.HasValue ? JsonSerializer.Deserialize<TStorage>(raw!, _jsonOptions) : default;
    }

    /// <inheritdoc />
    protected override Task AddStorageAsync(TStorage model)
    {
        var id = MapToDomain(model).Id;
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        return _db.StringSetAsync(Key(id), json);
    }

    /// <inheritdoc />
    protected override Task RemoveStorageAsync(string id) => _db.KeyDeleteAsync(Key(id));

    /// <inheritdoc />
    protected override Task AddRangeStorageAsync(IEnumerable<TStorage> models)
    {
        var entries = models.Select(m =>
        {
            var id = MapToDomain(m).Id;
            var json = JsonSerializer.Serialize(m, _jsonOptions);
            return new KeyValuePair<RedisKey, RedisValue>(Key(id), json);
        }).ToArray();

        return _db.StringSetAsync(entries);
    }

    /// <inheritdoc />
    protected override Task TruncateStorageAsync()
    {
        var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints()[0]);
        var keys = server.Keys(pattern: _prefix + "*").ToArray();
        return _db.KeyDeleteAsync(keys);
    }

    /// <inheritdoc />
    protected override TDomain MapToDomain(TStorage storage)
    {
        return storage.ToEntity();
    }

    /// <inheritdoc />
    protected override TStorage MapToStorage(TDomain domain)
    {
        var storage = new TStorage();
        return (TStorage)storage.ToStorageEntity(domain);
    }

    private string Key(string id) => _prefix + id;
}
