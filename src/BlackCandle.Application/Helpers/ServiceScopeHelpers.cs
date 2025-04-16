using BlackCandle.Application.Interfaces.Pipelines;

using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.Helpers;

/// <summary>
///     Помощь с ServiceScope
/// </summary>
internal static class ServiceScopeHelpers
{
    /// <summary>
    ///     Получить шаги
    /// </summary>
    public static IPipelineStep<TContext>[] GetPipelineSteps<TContext>(this IServiceScope scope)
        where TContext : new()
    {
        var steps = scope.ServiceProvider.GetServices<IPipelineStep<TContext>>();
        var pipelineSteps = steps.ToArray();
        if (pipelineSteps.Length == 0)
        {
            throw new InvalidOperationException($"Pipeline steps not found for {scope}");
        }

        return pipelineSteps;
    }
}
