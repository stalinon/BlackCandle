using BlackCandle.Application.Interfaces.Infrastructure;
using BlackCandle.Application.Interfaces.Pipelines;

using Microsoft.Extensions.DependencyInjection;

namespace BlackCandle.Application.Pipelines;

/// <inheritdoc cref="IPipelineFactory"/>
internal class PipelineFactory : IPipelineFactory
{
    private readonly IServiceProvider _provider;

    /// <inheritdoc cref="PipelineFactory"/>
    public PipelineFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc/>
    public TPipeline Create<TPipeline, TContext>()
        where TPipeline : Pipeline<TContext>
        where TContext : new()
    {
        using var scope = _provider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerService>();

        var instance = ActivatorUtilities.CreateInstance<TPipeline>(
            scope.ServiceProvider,
            scope,
            logger);

        return instance;
    }
}
