using BlackCandle.Application.Interfaces.Infrastructure;

using Microsoft.Extensions.Hosting;

using StackExchange.Redis;

namespace BlackCandle.Infrastructure.Persistence.Redis;

/// <summary>
///     Сервис проверки подключения к Redis
/// </summary>
public class RedisPingService : IHostedService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILoggerService _logger;

    /// <inheritdoc cref="RedisPingService" />
    public RedisPingService(IConnectionMultiplexer redis, ILoggerService logger)
    {
        _redis = redis;
        _logger = logger;
        _logger.AddPrefix("Redis");
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var pong = _redis.GetDatabase().Ping();
        _logger.LogInfo($"Ping = {pong.TotalMilliseconds} ms");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
