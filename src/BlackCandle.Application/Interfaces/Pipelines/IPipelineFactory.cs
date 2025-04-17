using BlackCandle.Application.Pipelines;

namespace BlackCandle.Application.Interfaces.Pipelines;

/// <summary>
///     Фабрика пайплайнов
/// </summary>
public interface IPipelineFactory
{
    /// <summary>
    ///     Создать пайплайн
    /// </summary>
    TPipeline Create<TPipeline, TContext>()
        where TPipeline : Pipeline<TContext>
        where TContext : new();
}
