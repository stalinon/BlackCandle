using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using IAuthenticationService = BlackCandle.Web.Interfaces.IAuthenticationService;

namespace BlackCandle.Web.Services;

/// <inheritdoc cref="IAuthenticationService"/>
internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly IConfiguration _config;

    /// <inheritdoc cref="AuthenticationService"/>
    public AuthenticationService(IHttpContextAccessor httpContext, IConfiguration config)
    {
        _httpContext = httpContext;
        _config = config;
    }

    /// <inheritdoc/>
    public async Task<bool> LoginAsync(string login, string password)
    {
        var expectedLogin = _config["Admin:Login"];
        var expectedPassword = _config["Admin:Password"];

        if (login != expectedLogin || password != expectedPassword)
        {
            return false;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, login),
            new(ClaimTypes.Role, "Admin"),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await _httpContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return true;
    }

    /// <inheritdoc/>
    public async Task LogoutAsync()
    {
        await _httpContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
