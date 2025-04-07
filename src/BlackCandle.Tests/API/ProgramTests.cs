using System.Net;

using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Infrastructure.Persistence.InMemory;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlackCandle.Tests.API;

/// <summary>
///     Smoke-тесты на <c>Program.cs</c>
/// </summary>
/// <remarks>
///     <list type="number">
///         <item>Приложение стартует</item>
///         <item>Маршрут отвечает</item>
///     </list>
/// </remarks>
public sealed class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    /// <inheritdoc cref="ProgramTests" />
    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    ///     Тест 1: Приложение стартует и возвращает 200 на /swagger/index.html
    /// </summary>
    [Fact(DisplayName = "Тест 1: Приложение стартует и возвращает 200 на /swagger/index.html")]
    public async Task App_ShouldStartAndServeSwagger()
    {
        // Act
        var response = await _client.GetAsync("/swagger/index.html");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    ///     Тест 2: При обращении к несуществующему маршруту возвращается ошибка 404
    /// </summary>
    [Fact(DisplayName = "Тест 2: При обращении к несуществующему маршруту возвращается ошибка 404")]
    public async Task App_ShouldReturn404_ForUnknownRoute()
    {
        // Act
        var response = await _client.GetAsync("/api/nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
