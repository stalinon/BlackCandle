using BlackCandle.Web.Interfaces;
using BlackCandle.Web.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

using MudBlazor.Services;

namespace BlackCandle.Web;

/// <summary>
///     Регистрация UI-компонентов и авторизации для админки
/// </summary>
public static class WebRegistration
{
    /// <summary>
    ///     Добавить сервисы админки
    /// </summary>
    public static IServiceCollection AddWebAdmin(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddAuthorizationCore();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/admin/login";
                options.AccessDeniedPath = "/admin/denied";
                options.Cookie.Name = "BlackCandle.Auth";
            });

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddHttpContextAccessor();

        return services;
    }
}
