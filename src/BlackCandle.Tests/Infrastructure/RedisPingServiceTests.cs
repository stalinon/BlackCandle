using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Infrastructure.Persistence.Redis;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using StackExchange.Redis;

namespace BlackCandle.Tests.Infrastructure;

/// <summary>
///     Тесты для <see cref="RedisPingService" />
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Ping вызывается при старте</item>
///         <item>StopAsync ничего не делает</item>
///     </list>
/// </remarks>
public sealed class RedisPingServiceTests
{
    /// <summary>
    ///     Тест 1: Redis ping вызывается при старте
    /// </summary>
    [Fact(DisplayName = "Тест 1: Redis ping вызывается при старте")]
    public async Task StartAsync_ShouldPingAndLog()
    {
        // Arrange
        var dbMock = new Mock<IDatabase>();
        dbMock.Setup(d => d.Ping(It.IsAny<CommandFlags>()))
              .Returns(TimeSpan.FromMilliseconds(42));

        var redisMock = new Mock<IConnectionMultiplexer>();
        redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                 .Returns(dbMock.Object);

        var loggerMock = new Mock<ILoggerService>();
        var service = new RedisPingService(redisMock.Object, CreateServiceScopeFactory(loggerMock));

        // Act
        await service.StartAsync(default);

        // Assert
        loggerMock.Verify(l => l.AddPrefix("Redis"), Times.Once);
        loggerMock.Verify(l => l.LogInfo(It.Is<string>(s => s.Contains("Ping = 42"))), Times.Once);
    }

    /// <summary>
    ///     Тест 2: StopAsync ничего не делает
    /// </summary>
    [Fact(DisplayName = "Тест 2: StopAsync ничего не делает")]
    public async Task StopAsync_ShouldDoNothing()
    {
        // Arrange
        var redis = new Mock<IConnectionMultiplexer>().Object;
        var service = new RedisPingService(redis, CreateServiceScopeFactory());

        // Act
        var result = service.StopAsync(default);
        await result;

        // Assert
        result.Should().Be(Task.CompletedTask);
    }

    private static IServiceScopeFactory CreateServiceScopeFactory(Mock<ILoggerService>? loggerMock = null)
    {
        loggerMock ??= new Mock<ILoggerService>();
        var logger = loggerMock.Object;
        var mock = new Mock<IServiceScopeFactory>();
        var mockScope = new Mock<IServiceScope>();
        var mockProvider = new Mock<IServiceProvider>();
        mockProvider.Setup(p => p.GetService(typeof(ILoggerService))).Returns(logger);

        mockScope.Setup(s => s.ServiceProvider).Returns(mockProvider.Object);
        mock.Setup(s => s.CreateScope()).Returns(mockScope.Object);
        return mock.Object;
    }
}
