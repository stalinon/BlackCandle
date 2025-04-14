using System.Security.Claims;

using BlackCandle.Web.Services;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

namespace BlackCandle.Tests.Web;

/// <summary>
///     Тесты для <see cref="BlazorCookieAuthProvider"/>
/// </summary>
public sealed class BlazorCookieAuthProviderTests
{
    /// <summary>
    ///     Возвращает пользователя из контекста
    /// </summary>
    [Fact(DisplayName = "Возвращает пользователя из контекста")]
    public async Task GetAuthenticationStateAsync_Should_Return_Context_User()
    {
        // Arrange
        var expectedClaims = new[]
        {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "Admin"),
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(expectedClaims, "cookie"));

        var context = new DefaultHttpContext { User = principal };

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

        var provider = new BlazorCookieAuthProvider(httpContextAccessor.Object);

        // Act
        var result = await provider.GetAuthenticationStateAsync();

        // Assert
        result.User.Identity.Should().NotBeNull();
        result.User.Identity!.IsAuthenticated.Should().BeTrue();
        result.User.Claims
            .Select(c => (c.Type, c.Value))
            .Should()
            .BeEquivalentTo(expectedClaims.Select(c => (c.Type, c.Value)));
    }

    /// <summary>
    ///     Возвращает пустого пользователя при отсутствии контекста
    /// </summary>
    [Fact(DisplayName = "Возвращает пустого пользователя при отсутствии контекста")]
    public async Task GetAuthenticationStateAsync_Should_Return_Empty_If_Context_Null()
    {
        // Arrange
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        var provider = new BlazorCookieAuthProvider(httpContextAccessor.Object);

        // Act
        var result = await provider.GetAuthenticationStateAsync();

        // Assert
        result.User.Identity.Should().NotBeNull();
        result.User.Identity!.IsAuthenticated.Should().BeFalse();
        result.User.Claims.Should().BeEmpty();
    }
}
