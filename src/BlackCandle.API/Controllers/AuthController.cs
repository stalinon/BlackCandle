using System.Security.Claims;

using BlackCandle.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.API.Controllers;

/// <summary>
///     Контроллер авторизации
/// </summary>
[ApiController]
[Route(AuthRoutes.Base)]
public class AuthController(IConfiguration config) : ControllerBase
{
    private const string Scheme = AuthRoutes.AuthScheme;
    private const string AdminRole = AuthRoutes.AdminRole;

    /// <summary>
    ///     Вход
    /// </summary>
    [HttpPost(AuthRoutes.Login)]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromForm] string login, [FromForm] string password)
    {
        var expectedLogin = config["AdminAuth:Login"];
        var expectedPassword = config["AdminAuth:Password"];

        if (login != expectedLogin || password != expectedPassword)
        {
            return Redirect(AuthRoutes.LoginRedirectError);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, login),
            new(ClaimTypes.Role, AdminRole),
        };

        var identity = new ClaimsIdentity(claims, Scheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(Scheme, principal);

        return Redirect(AuthRoutes.LoginRedirectSuccess);
    }

    /// <summary>
    ///     Выход
    /// </summary>
    [HttpPost(AuthRoutes.Logout)]
    [Authorize(Roles = AdminRole)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(Scheme);
        return Redirect(AuthRoutes.LoginPage);
    }
}
