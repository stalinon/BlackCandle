using BlackCandle.Web.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Web;

/// <summary>
///     Регистрация сервисов Web
/// </summary>
public static class WebRegistration
{
    /// <summary>
    ///     Зарегистрировать
    /// </summary>
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = "AdminAuthScheme";
            })
            .AddCookie("AdminAuthScheme", options =>
            {
                options.Cookie.Name = "BlackCandleAuth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/denied";
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<AuthenticationStateProvider, BlazorCookieAuthProvider>();

        return services;
    }
}
