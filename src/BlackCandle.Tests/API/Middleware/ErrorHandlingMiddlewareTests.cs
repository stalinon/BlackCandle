using System.Net;

using BlackCandle.API.Middleware;
using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Domain.Exceptions;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using MemoryStream = System.IO.MemoryStream;

namespace BlackCandle.Tests.API.Middleware;

/// <summary>
///     Тесты на <see cref="ErrorHandlingMiddleware"/>
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Обрабатывает BlackCandleException как 400</item>
///         <item>Обрабатывает Exception как 500</item>
///         <item>Пропускает запрос без исключений</item>
///     </list>
/// </remarks>
public sealed class ErrorHandlingMiddlewareTests
{
    private readonly Mock<ILoggerService> _loggerMock = new();

    /// <inheritdoc cref="ErrorHandlingMiddlewareTests" />
    public ErrorHandlingMiddlewareTests() { }

    /// <summary>
    ///     Тест 1: Обрабатывает BlackCandleException как 400
    /// </summary>
    [Fact(DisplayName = "Тест 1: Обрабатывает BlackCandleException как 400")]
    public async Task ShouldHandleBlackCandleExceptionAsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream(),
            },
        };
        var middleware = new ErrorHandlingMiddleware(_ => throw new BlackCandleException("bad-request"), CreateServiceScopeFactory());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        var body = await ReadResponseBody(context);
        body.Should().Contain("bad-request");

        _loggerMock.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    /// <summary>
    ///     Тест 2: Обрабатывает Exception как 500
    /// </summary>
    [Fact(DisplayName = "Тест 2: Обрабатывает Exception как 500")]
    public async Task ShouldHandleGenericExceptionAsInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream(),
            },
        };
        var middleware = new ErrorHandlingMiddleware(_ => throw new Exception("fatal"), CreateServiceScopeFactory());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

        var body = await ReadResponseBody(context);
        body.Should().Contain("fatal");

        _loggerMock.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
    }

    /// <summary>
    ///     Тест 3: Пропускает запрос без исключений
    /// </summary>
    [Fact(DisplayName = "Тест 3: Пропускает запрос без исключений")]
    public async Task ShouldPassThroughWithoutException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var middleware = new ErrorHandlingMiddleware(
            _ =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                return Task.CompletedTask;
            }, CreateServiceScopeFactory());

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        _loggerMock.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
    }

    private static async Task<string> ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }

    private IServiceScopeFactory CreateServiceScopeFactory()
    {
        var mock = new Mock<IServiceScopeFactory>();
        var mockScope = new Mock<IServiceScope>();
        var mockProvider = new Mock<IServiceProvider>();
        mockProvider.Setup(p => p.GetService(typeof(ILoggerService))).Returns(_loggerMock.Object);

        mockScope.Setup(s => s.ServiceProvider).Returns(mockProvider.Object);
        mock.Setup(s => s.CreateScope()).Returns(mockScope.Object);
        return mock.Object;
    }
}
