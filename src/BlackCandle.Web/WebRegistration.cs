using BlackCandle.Web.Components;
using BlackCandle.Web.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MudBlazor.Services;

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
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        services.AddMudServices();
        services.AddRazorPages();
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = AuthRoutes.AuthScheme;
            })
            .AddCookie(AuthRoutes.AuthScheme, options =>
            {
                options.Cookie.Name = AuthRoutes.AuthScheme;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.LoginPath = AuthRoutes.LoginPage;
            });

        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<AuthenticationStateProvider, BlazorCookieAuthProvider>();

        return services;
    }

    /// <summary>
    ///     Зарегистрировать
    /// </summary>
    public static IApplicationBuilder UseWebServices(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAntiforgery();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        app.MapRazorPages();

        return app;
    }
}
