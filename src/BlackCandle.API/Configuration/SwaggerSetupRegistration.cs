using Microsoft.OpenApi.Models;

namespace BlackCandle.API.Configuration;

/// <summary>
///     Конфигурация Swagger-документации.
/// </summary>
public static class SwaggerSetupRegistration
{
    /// <summary>
    ///     Добавить документацию Swagger
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "BlackCandle API",
                Version = "v1",
                Description = "Финансовый движок тьмы.",
            });
        });

        return services;
    }

    /// <summary>
    ///     Использовать документацию Swagger
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlackCandle API V1"); });

        return app;
    }
}
