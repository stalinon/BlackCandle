using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlackCandle.API.Controllers;

/// <summary>
///     Контроллер авторизации
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController(IConfiguration config) : ControllerBase
{
    /// <summary>
    ///     Вход
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromForm] string login, [FromForm] string password)
    {
        var expectedLogin = config["AdminAuth:Login"];
        var expectedPassword = config["AdminAuth:Password"];

        if (login != expectedLogin || password != expectedPassword)
        {
            return Redirect("/login?error=1");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, login),
            new(ClaimTypes.Role, "Admin"),
        };

        var identity = new ClaimsIdentity(claims, "AdminAuthScheme");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("AdminAuthScheme", principal);

        return Redirect("/");
    }

    /// <summary>
    ///     Выход
    /// </summary>
    [HttpPost("logout")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("AdminAuthScheme");
        return Redirect("/login");
    }
}
