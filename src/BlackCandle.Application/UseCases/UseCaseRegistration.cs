using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.UseCases;

/// <summary>
///     Регистрация Use-case
/// </summary>
public static class UseCaseRegistration
{
    /// <summary>
    ///     Регистрация Use-case
    /// </summary>
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        var assembly = Assembly.GetAssembly(typeof(UseCaseRegistration))!;

        var interfaceTypes = new[]
        {
            typeof(Abstractions.IUseCase<>),
            typeof(Abstractions.IUseCase<,>),
        };

        foreach (var type in assembly.GetTypes())
        {
            if (type is not { IsAbstract: false, IsInterface: false })
            {
                continue;
            }

            var interfaces = type.GetInterfaces()
                .Where(i =>
                    i.IsGenericType &&
                    interfaceTypes.Contains(i.GetGenericTypeDefinition()));

            foreach (var i in interfaces)
            {
                services.AddScoped(i, type);
            }
        }

        return services;
    }
}
