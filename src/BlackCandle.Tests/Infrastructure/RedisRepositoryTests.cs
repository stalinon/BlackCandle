using System.Net;
using System.Text.Json;

using BlackCandle.Domain.Configuration;
using BlackCandle.Domain.Entities;
using BlackCandle.Domain.Enums;
using BlackCandle.Domain.Interfaces;
using BlackCandle.Infrastructure.Persistence.Redis;
using BlackCandle.Infrastructure.Persistence.Redis.Entities;

using FluentAssertions;

using Moq;

using StackExchange.Redis;

namespace BlackCandle.Tests.Infrastructure;

/// <summary>
///     Тесты для <see cref="RedisRepository{TDomain,TStorage}" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Получение сущностей с/без фильтра</item>
///         <item>Получение по ключу</item>
///         <item>Сохранение и сериализация</item>
///         <item>Удаление и очистка</item>
///     </list>
/// </remarks>
public sealed class RedisRepositoryTests
{
    private const string Prefix = "test:";
    private readonly Mock<IConnectionMultiplexer> _multiplexerMock = new();
    private readonly Mock<IDatabase> _dbMock = new();
    private readonly Mock<IServer> _serverMock = new();

    private readonly RedisRepository<TradeSignal, RedisTradeSignal> _repository;

    /// <inheritdoc cref="RedisRepositoryTests" />
    public RedisRepositoryTests()
    {
        var endpoint = new DnsEndPoint("localhost", 6379);

        _multiplexerMock.Setup(x => x.GetDatabase(It.IsAny<int>(), null)).Returns(_dbMock.Object);
        _multiplexerMock.Setup(x => x.GetEndPoints(It.IsAny<bool>())).Returns([endpoint]);
        _multiplexerMock.Setup(x => x.GetServer(endpoint, It.IsAny<object>())).Returns(_serverMock.Object);
        _dbMock.SetupGet(db => db.Multiplexer).Returns(_multiplexerMock.Object);

        _repository = new RedisRepository<TradeSignal, RedisTradeSignal>(_multiplexerMock.Object, new RedisOptions
        {
            Prefix = Prefix,
        });
    }

    /// <summary>
    ///     Тест 1: Получение сущностей без фильтра
    /// </summary>
    [Fact(DisplayName = "Тест 1: Получение сущностей без фильтра")]
    public async Task GetAllAsync_ShouldReturnAll_WhenNoFilter()
    {
        // Arrange
        var keys = new RedisKey[] { "test:1" };
        var redisEntity = new RedisTradeSignal
        {
            Ticker = new Ticker { Symbol = "AAPL" },
            Action = TradeAction.Buy,
            Confidence = ConfidenceLevel.Medium,
            Reason = "logic",
            Date = DateTime.Today,
            FundamentalScore = 90,
        };
        var json = JsonSerializer.Serialize(redisEntity);
        var values = new RedisValue[] { json };

        _serverMock.Setup(s => s.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(keys);
        _dbMock.Setup(d => d.StringGetAsync(keys, CommandFlags.None)).ReturnsAsync(values);

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().ContainSingle();
        result[0].Ticker.Symbol.Should().Be("AAPL");
    }

    /// <summary>
    ///     Тест 2: Получение сущностей с фильтром
    /// </summary>
    [Fact(DisplayName = "Тест 2: Получение сущностей с фильтром")]
    public async Task GetAllAsync_ShouldApplyFilter()
    {
        // Arrange
        var keys = new RedisKey[] { "test:1" };
        var entity = new RedisTradeSignal
        {
            Ticker = new Ticker { Symbol = "AAPL" },
            Action = TradeAction.Buy,
            Confidence = ConfidenceLevel.Medium,
            Reason = "logic",
            Date = DateTime.Today,
            FundamentalScore = 90,
        };
        var json = JsonSerializer.Serialize(entity);
        var values = new RedisValue[] { json };

        var filterMock = new Mock<IFilter<TradeSignal>>();
        filterMock.Setup(f => f.Apply(It.IsAny<IQueryable<TradeSignal>>()))
            .Returns<IQueryable<TradeSignal>>(q => q.Where(x => x.Action == TradeAction.Buy));

        _serverMock.Setup(s => s.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(keys);
        _dbMock.Setup(d => d.StringGetAsync(keys, CommandFlags.None)).ReturnsAsync(values);

        // Act
        var result = await _repository.GetAllAsync(filterMock.Object);

        // Assert
        result.Should().HaveCount(1);
        result[0].Action.Should().Be(TradeAction.Buy);
    }

    /// <summary>
    ///     Тест 3: Получение по идентификатору
    /// </summary>
    [Fact(DisplayName = "Тест 3: Получение по идентификатору")]
    public async Task GetByIdAsync_ShouldReturnEntity()
    {
        // Arrange
        var id = "1";
        var redisEntity = new RedisTradeSignal
        {
            Ticker = new Ticker { Symbol = "AAPL" },
            Action = TradeAction.Buy,
            Confidence = ConfidenceLevel.Medium,
            Reason = "logic",
            Date = DateTime.Today,
            FundamentalScore = 90,
        };
        var json = JsonSerializer.Serialize(redisEntity);

        _dbMock.Setup(d => d.StringGetAsync(Prefix + nameof(TradeSignal).ToLower() + ":" + id, CommandFlags.None)).ReturnsAsync(json);

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(redisEntity.ToEntity().Id);
    }

    /// <summary>
    ///     Тест 4: Добавление сущности
    /// </summary>
    [Fact(DisplayName = "Тест 4: Добавление сущности")]
    public async Task AddAsync_ShouldSerializeAndStore()
    {
        // Arrange
        var entity = new TradeSignal
        {
            Ticker = new Ticker { Symbol = "AAPL" },
            Action = TradeAction.Sell,
            Confidence = ConfidenceLevel.High,
            Reason = "mock",
            Date = DateTime.Today,
            FundamentalScore = 80,
        };

        // Act
        await _repository.AddAsync(entity);

        // Assert
        _dbMock.Verify(
            d => d.StringSetAsync(
            Prefix + nameof(TradeSignal).ToLower() + ":" + entity.Id,
            It.IsAny<RedisValue>(),
            null, false, When.Always, CommandFlags.None), Times.Once);
    }

    /// <summary>
    ///     Тест 5: Удаление по ключу
    /// </summary>
    [Fact(DisplayName = "Тест 5: Удаление по ключу")]
    public async Task RemoveAsync_ShouldDeleteKey()
    {
        // Arrange
        const string id = "to-delete";

        // Act
        await _repository.RemoveAsync(id);

        // Assert
        _dbMock.Verify(d => d.KeyDeleteAsync(Prefix + nameof(TradeSignal).ToLower() + ":" + id, CommandFlags.None), Times.Once);
    }

    /// <summary>
    ///     Тест 6: Очистка всех сущностей
    /// </summary>
    [Fact(DisplayName = "Тест 6: Очистка всех сущностей")]
    public async Task TruncateAsync_ShouldDeleteAllWithPrefix()
    {
        // Arrange
        var keys = new RedisKey[] { Prefix + "1", Prefix + "2" };
        _serverMock.Setup(
                s => s.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(keys);

        // Act
        await _repository.TruncateAsync();

        // Assert
        _dbMock.Verify(d => d.KeyDeleteAsync(keys, CommandFlags.None), Times.Once);
    }
}
