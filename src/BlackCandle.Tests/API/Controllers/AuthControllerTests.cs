using System.Security.Claims;

using BlackCandle.API.Controllers;
using BlackCandle.Web;

using FluentAssertions;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using Moq;

namespace BlackCandle.Tests.API.Controllers;

/// <summary>
///     Тесты для <see cref="AuthController" />
/// </summary>
public sealed class AuthControllerTests
{
    /// <summary>
    ///     Успешный вход
    /// </summary>
    [Fact(DisplayName = "Успешный вход")]
    public async Task Login_Should_Redirect_To_Root_On_Success()
    {
        // Arrange
        const string expectedLogin = "admin";
        const string expectedPassword = "password";
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminAuth:Login"] = expectedLogin,
                ["AdminAuth:Password"] = expectedPassword,
            })
            .Build();

        var authMock = new Mock<IAuthenticationService>();
        var context = new DefaultHttpContext
        {
            RequestServices = MockAuthServices(authMock.Object),
        };

        var controller = new AuthController(config)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = context,
            },
        };

        // Act
        var result = await controller.Login(expectedLogin, expectedPassword);

        // Assert
        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Be(AuthRoutes.LoginRedirectSuccess);

        authMock.Verify(
            x =>
                x.SignInAsync(
                    context,
                    AuthRoutes.AuthScheme,
                    It.Is<ClaimsPrincipal>(c => c.Identity!.Name == expectedLogin),
                    It.IsAny<AuthenticationProperties>()),
            Times.Once);
    }

    /// <summary>
    ///     Ошибка при входе
    /// </summary>
    [Fact(DisplayName = "Ошибка при входе")]
    public async Task Login_Should_Redirect_To_Error_On_Failure()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminAuth:Login"] = "admin",
                ["AdminAuth:Password"] = "password",
            })
            .Build();

        var authMock = new Mock<IAuthenticationService>();
        var context = new DefaultHttpContext();
        context.RequestServices = MockAuthServices(authMock.Object);

        var controller = new AuthController(config)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = context,
            },
        };

        // Act
        var result = await controller.Login("wrong", "user");

        // Assert
        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Be(AuthRoutes.LoginRedirectError);

        authMock.Verify(
            x =>
                x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()),
            Times.Never);
    }

    /// <summary>
    ///     Успешный выход
    /// </summary>
    /// <remarks>
    ///     <list type="number">
    ///         <item>Выполняется SignOutAsync</item>
    ///         <item>Происходит редирект на /login</item>
    ///     </list>
    /// </remarks>
    [Fact(DisplayName = "Успешный выход")]
    public async Task Logout_Should_Redirect_To_Login()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var authMock = new Mock<IAuthenticationService>();

        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                new Claim(ClaimTypes.Role, AuthRoutes.AdminRole)
            ], "cookie")),
            RequestServices = MockAuthServices(authMock.Object),
        };

        var controller = new AuthController(config)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = context,
            },
        };

        // Act
        var result = await controller.Logout();

        // Assert
        result.Should().BeOfType<RedirectResult>()
            .Which.Url.Should().Be(AuthRoutes.LoginPage);

        authMock.Verify(
            x => x.SignOutAsync(context, AuthRoutes.AuthScheme, It.IsAny<AuthenticationProperties>()),
            Times.Once);
    }

    private static IServiceProvider MockAuthServices(IAuthenticationService authService)
    {
        var serviceMock = new Mock<IServiceProvider>();
        serviceMock.Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(authService);
        return serviceMock.Object;
    }
}
